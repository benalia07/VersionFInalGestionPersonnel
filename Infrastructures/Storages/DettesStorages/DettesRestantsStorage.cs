using GestionPersonnel.Models.Salaires;
using Infrastructures.Domains.Models.Dettes;
using Infrastructures.Storages.DettesStorages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace GestionPersonnel.Storages.DettesStorages
{
    public class DetteRestantStorage : IDetteRestantStorage
    {
        private readonly string _connectionString;

        public DetteRestantStorage(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DBConnection");
        }

        private const string _selectAllQuery = "SELECT * FROM DettesRestants";
        private const string _selectByIdQuery = "SELECT * FROM DettesRestants WHERE IdDettesRestants = @id";
        private const string _insertQuery = "INSERT INTO DettesRestants (EmployeId, DettesRestants) VALUES (@EmployeId, @DettesRestants); SELECT SCOPE_IDENTITY();";
        private const string _updateQuery = "UPDATE DettesRestants SET EmployeId = @EmployeId, DettesRestants = @DettesRestants WHERE IdDettesRestants = @IdDettesRestants;";
        private const string _deleteQuery = "DELETE FROM DettesRestants WHERE IdDettesRestants = @IdDettesRestants;";

        private static DettesRestants GetDetteRestantFromDataRow(DataRow row)
        {
            return new DettesRestants
            {
                IdDettesRestants = (int)row["IdDettesRestants"],
                EmployeId = (int)row["EmployeId"],
                DettesRestants = (decimal)row["DettesRestants"]
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
        public async Task<List<DettesRestants>> GetByEmployeIdAsync(int employeId)
        {
            const string query = "SELECT IdDettesRestants, EmployeId, DettesRestants FROM DettesRestants WHERE EmployeId = @EmployeId";

            var dettesRestantes = new List<DettesRestants>();

            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(query, connection);
            cmd.Parameters.AddWithValue("@EmployeId", employeId);

            connection.Open();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                var dette = new DettesRestants
                {
                    IdDettesRestants = reader.GetInt32(0),
                    EmployeId = reader.GetInt32(1),
                    DettesRestants = reader.GetDecimal(2)
                };

                dettesRestantes.Add(dette);
            }

            return dettesRestantes;
        }

        public async Task<List<DettesRestants>> GetAll()
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_selectAllQuery, connection);

            DataTable dataTable = new();
            SqlDataAdapter da = new(cmd);

            connection.Open();
            da.Fill(dataTable);

            return (from DataRow row in dataTable.Rows select GetDetteRestantFromDataRow(row)).ToList();
        }

        public async Task<DettesRestants?> GetById(int id)
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

        public async Task Add(DettesRestants detteRestant)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_insertQuery, connection);
            cmd.Parameters.AddWithValue("@EmployeId", detteRestant.EmployeId);
            cmd.Parameters.AddWithValue("@DettesRestants", detteRestant.DettesRestants);

            connection.Open();
            var id = await cmd.ExecuteScalarAsync();
            detteRestant.IdDettesRestants = Convert.ToInt32(id);
        }

        public async Task Update(DettesRestants detteRestant)
        {
            await using var connection = new SqlConnection(_connectionString);
            SqlCommand cmd = new(_updateQuery, connection);
            cmd.Parameters.AddWithValue("@EmployeId", detteRestant.EmployeId);
            cmd.Parameters.AddWithValue("@DettesRestants", detteRestant.DettesRestants);
            cmd.Parameters.AddWithValue("@IdDettesRestants", detteRestant.IdDettesRestants);

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
    }

}
