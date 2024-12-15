using GestionPersonnel.Models.Salaires;
using GestionPersonnel.Models.Dettes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GestionPersonnel.Storages.DettesStorages
{
    public class DetteRestantStorage
    {
        private readonly string _connectionString;

        public DetteRestantStorage(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DBConnection");
        }

        private const string _selectAllQuery = "SELECT * FROM DettesRestants";
        private const string _selectByIdQuery = "SELECT * FROM DettesRestants WHERE IdDettesRestants = @id";
        private const string _selectById2Query = "SELECT * FROM DettesRestants WHERE EmployeId = @id";

        private const string _insertQuery = "INSERT INTO DettesRestants (EmployeId, DettesRestants) VALUES (@EmployeId, @DettesRestants); ";
        private const string _update2Query = "UPDATE DettesRestants SET  DettesRestants = @DettesRestants WHERE EmployeId = @EmployeId;";
        private const string montantretirerQuery = "UPDATE DettesRestants SET  DettesRestants = @DettesRestants WHERE EmployeId = @EmployeId;";

        private const string _updateQuery = "UPDATE DettesRestants SET  DettesRestants = @DettesRestants WHERE IdDettesRestants = @IdDettesRestants;";
        private const string _deleteQuery = "DELETE FROM DettesRestants WHERE IdDettesRestants = @IdDettesRestants;";

        private static DetteRestant GetDetteRestantFromDataRow(DataRow row)
        {
            return new DetteRestant
            {
                IdDettesRestants = row["IdDettesRestants"] !=DBNull.Value? (int)row["IdDettesRestants"]:0,
                EmployeId = row["EmployeId"]!=DBNull.Value?(int)row["EmployeId"]:0,
                DettesRestants = row["DettesRestants"]!=DBNull.Value?(decimal)row["DettesRestants"]:0
            };
        }
        public async Task<bool> ExisteDettePourEmploye(int employeId)
        {
            const string query = "SELECT COUNT(1) FROM DettesRestants WHERE EmployeId = @EmployeId";

            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(query, connection);
            cmd.Parameters.AddWithValue("@EmployeId", employeId);

            connection.Open();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }
        public async Task<List<DetteRestant>> GetByEmployeIdAsync(int employeId)
        {
            const string query = "SELECT IdDettesRestants, EmployeId, DettesRestants FROM DettesRestants WHERE EmployeId = @EmployeId";

            var dettesRestantes = new List<DetteRestant>();

            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(query, connection);
            cmd.Parameters.AddWithValue("@EmployeId", employeId);

            connection.Open();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                var dette = new DetteRestant
                {
                    IdDettesRestants = reader.GetInt32(0),
                    EmployeId = reader.GetInt32(1),
                    DettesRestants = reader.GetDecimal(2)
                };

                dettesRestantes.Add(dette);
            }

            return dettesRestantes;
        }

        public async Task<List<DetteRestant>> GetAll()
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_selectAllQuery, connection);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            connection.Open();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetDetteRestantFromDataRow(row)).ToList();
        }

        public async Task<DetteRestant?> GetById(int id)
        {
            await using var connection = new SqlConnection(_connectionString);

            SqlCommand cmd = new(_selectByIdQuery, connection);
            cmd.Parameters.AddWithValue("@id", id);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            connection.Open();
            da.Fill(dataTable);

            return dataTable.Rows.Count == 0 ? null : GetDetteRestantFromDataRow(dataTable.Rows[0]);
        }
        public async Task<DetteRestant?> GetById2(int id)
        {
            await using var connection = new SqlConnection(_connectionString);

            SqlCommand cmd = new(_selectById2Query, connection);
            cmd.Parameters.AddWithValue("@id", id);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            connection.Open();
            da.Fill(dataTable);

            return dataTable.Rows.Count == 0 ? null : GetDetteRestantFromDataRow(dataTable.Rows[0]);
        }

        public async Task Add(DetteRestant detteRestant)
        {
            if ((await ExisteDettePourEmploye(detteRestant.EmployeId)))
            {
                var detteRestants2 = new DetteRestant();
                detteRestants2 = await GetById2(detteRestant.EmployeId);
                decimal somme = detteRestant.DettesRestants + detteRestants2.DettesRestants;
                await using var connection = new SqlConnection(_connectionString);
                SqlCommand cmd = new(_update2Query, connection);
                cmd.Parameters.AddWithValue("@EmployeId", detteRestant.EmployeId);
                cmd.Parameters.AddWithValue("@DettesRestants", somme);

                connection.Open();
                await cmd.ExecuteNonQueryAsync();

            }
            else
            {
                await using var connection = new SqlConnection(_connectionString);
                SqlCommand cmd = new(_insertQuery, connection);
                cmd.Parameters.AddWithValue("@EmployeId", detteRestant.EmployeId);
                cmd.Parameters.AddWithValue("@DettesRestants", detteRestant.DettesRestants);

                connection.Open();
                var id = await cmd.ExecuteScalarAsync();
                detteRestant.IdDettesRestants = Convert.ToInt32(id);
            }
        }

        public async Task Update(DetteRestant detteRestant)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_updateQuery, connection);
            cmd.Parameters.AddWithValue("@EmployeId", detteRestant.EmployeId);
            cmd.Parameters.AddWithValue("@DettesRestants", detteRestant.DettesRestants);
            cmd.Parameters.AddWithValue("@DettesRestants", detteRestant.IdDettesRestants);

            connection.Open();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(int id)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_deleteQuery, connection);
            cmd.Parameters.AddWithValue("@IdDettesRestants", id);

            connection.Open();
            await cmd.ExecuteNonQueryAsync();
        }
        public async Task MontantRetirer(int employeid,decimal montant)
        {
            var detteRestants2 = new DetteRestant();
            detteRestants2 = await GetById2(employeid);
            decimal somme =   detteRestants2.DettesRestants- montant;
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_update2Query, connection);
            cmd.Parameters.AddWithValue("@EmployeId", employeid);
            cmd.Parameters.AddWithValue("@DettesRestants", somme);

            connection.Open();
            await cmd.ExecuteNonQueryAsync();
        }
    }

}
