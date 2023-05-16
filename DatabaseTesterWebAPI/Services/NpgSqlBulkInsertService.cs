using DatabaseTesterWebAPI.Models;
using DatabaseTests.Models;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using Serilog;
using System.Reflection;
using System.Text;

namespace DatabaseTesterWebAPI.Services
{
    public interface INpgSqlBulkInsertService
    {
        public void WriteToServer<T>(IEnumerable<T> data, string destinationTable);
        public Task BulkInsertBinaryImporter(IEnumerable<User> teachers);
    }

    public class NpgSqlBulkInsertService : INpgSqlBulkInsertService
    {
        private readonly NpgSqlConfig _npgSqlConfig;

        public NpgSqlBulkInsertService(IOptions<NpgSqlConfig> npgSqlConfig)
        {
            _npgSqlConfig = npgSqlConfig.Value;
        }

        // https://stackoverflow.com/a/65701069/21887177
        public void WriteToServer<T>(IEnumerable<T> data, string destinationTable)
        {
            using var dbConnection = new NpgsqlConnection(_npgSqlConfig.ConnectionString);
            dbConnection.Open();

            try
            {
                if (destinationTable == null || destinationTable == string.Empty)
                {
                    throw new ArgumentOutOfRangeException("DestinationTableName", "Destination table must be set");
                }
                PropertyInfo[] properties = typeof(T).GetProperties();
                int colCount = properties.Length;

                NpgsqlDbType[] types = new NpgsqlDbType[colCount];
                int[] lengths = new int[colCount];
                string[] fieldNames = new string[colCount];
                
                var alteUnlogged = new NpgsqlCommand($"ALTER TABLE {destinationTable} SET UNLOGGED");
                alteUnlogged.ExecuteNonQuery();

                using (var cmd = new NpgsqlCommand($"SELECT * FROM {destinationTable} LIMIT 1", dbConnection))
                {
                    using var rdr = cmd.ExecuteReader();
                    if (rdr.FieldCount != colCount)
                    {
                        throw new ArgumentOutOfRangeException("dataTable", "Column count in Destination Table does not match column count in source table.");
                    }
                    var columns = rdr.GetColumnSchema();
                    for (int i = 0; i < colCount; i++)
                    {
                        types[i] = (NpgsqlDbType)columns[i].NpgsqlDbType;
                        lengths[i] = columns[i].ColumnSize == null ? 0 : (int)columns[i].ColumnSize;
                        fieldNames[i] = columns[i].ColumnName;
                    }

                }
                var sB = new StringBuilder($"\"{fieldNames[0]}\"");
                for (int p = 1; p < colCount; p++)
                {
                    sB.Append(", " + $"\"{fieldNames[p]}\"");
                }

                using var writer = dbConnection.BeginBinaryImport($"COPY {destinationTable} ({sB}) FROM STDIN (FORMAT BINARY)");
                foreach (var t in data)
                {
                    writer.StartRow();

                    for (int i = 0; i < colCount; i++)
                    {
                        if (properties[i].GetValue(t) == null)
                        {
                            writer.WriteNull();
                        }
                        else
                        {
                            switch (types[i])
                            {
                                case NpgsqlDbType.Date:
                                    writer.Write((DateTime)properties[i].GetValue(t), types[i]);
                                    break;
                                case NpgsqlDbType.Double:
                                    writer.Write((double)properties[i].GetValue(t), types[i]);
                                    break;
                                case NpgsqlDbType.Integer:
                                    try
                                    {
                                        var type = properties[i].GetType();
                                        if (properties[i].PropertyType.FullName == typeof(int).ToString())
                                        {
                                            writer.Write((int)properties[i].GetValue(t), types[i]);
                                            break;
                                        }
                                        else if (properties[i].PropertyType.FullName == typeof(string).ToString())
                                        {
                                            var swap = Convert.ToInt32(properties[i].GetValue(t));
                                            writer.Write((int)swap, types[i]);
                                            break;
                                        }
                                        else if (properties[i].GetType() == typeof(object))
                                        {

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        string sh = ex.Message;
                                    }

                                    writer.Write((object)properties[i].GetValue(t), types[i]);
                                    break;
                            }
                        }
                    }
                }
                writer.Complete();

                var alteLogged = new NpgsqlCommand($"ALTER TABLE {destinationTable} SET LOGGED");
                alteUnlogged.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing NpgSqlBulkCopy.WriteToServer().  See inner exception for details", ex);
            }
        }
        public async Task BulkInsertBinaryImporter(IEnumerable<User> users)
        {
            PropertyInfo[] properties = typeof(User).GetProperties();
            int colCount = properties.Length;

            var sB = new StringBuilder($"\"{properties[1].Name}\"");
            for (int p = 2; p < colCount; p++)
            {
                sB.Append(", " + $"\"{properties[p].Name}\"");
            }

            using var dbUsrConnection = new NpgsqlConnection(_npgSqlConfig.ConnectionString);
            using var dbAddrConnection = new NpgsqlConnection(_npgSqlConfig.ConnectionString);
            dbUsrConnection.Open();
            dbAddrConnection.Open();

            NpgsqlCommand cmd = new NpgsqlCommand("select nextval(pg_get_serial_sequence('public.\"Addresses\"', 'AddressId'))", dbAddrConnection);
            foreach (User p in users.Where(x => x.Address.AddressId == 0))
            {
                p.Address.AddressId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            using var usrWriter = dbUsrConnection.BeginBinaryImport($"COPY public.\"Users\" (\"FirstName\", \"LastName\", \"UserName\", \"Password\", \"Email\", \"AddressId\") FROM STDIN (FORMAT BINARY)");
            using var addrWriter = dbAddrConnection.BeginBinaryImport($"COPY public.\"Addresses\" (\"StreetName\", \"BuildingNumber\", \"ApartmentNumber\", \"PostalCode\", \"City\", \"Country\") FROM STDIN (FORMAT BINARY)");
            foreach (var user in users)
            {
                await addrWriter.StartRowAsync().ConfigureAwait(false);
                await addrWriter.WriteAsync(user.Address.StreetName, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await addrWriter.WriteAsync(user.Address.BuildingNumber, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await addrWriter.WriteAsync(user.Address.ApartmentNumber, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await addrWriter.WriteAsync(user.Address.PostalCode, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await addrWriter.WriteAsync(user.Address.City, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await addrWriter.WriteAsync(user.Address.Country, NpgsqlDbType.Varchar).ConfigureAwait(false);

                await usrWriter.StartRowAsync().ConfigureAwait(false);
                await usrWriter.WriteAsync(user.FirstName, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await usrWriter.WriteAsync(user.LastName, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await usrWriter.WriteAsync(user.UserName, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await usrWriter.WriteAsync(user.Password, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await usrWriter.WriteAsync(user.Email, NpgsqlDbType.Varchar).ConfigureAwait(false);
                await usrWriter.WriteAsync(user.Address.AddressId, NpgsqlDbType.Integer).ConfigureAwait(false);
            }
            await addrWriter.CompleteAsync();
            await usrWriter.CompleteAsync();
        }
    }
}
