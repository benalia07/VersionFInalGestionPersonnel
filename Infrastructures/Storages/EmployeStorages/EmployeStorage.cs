﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestionPersonnel.Models.Employe;
using GestionPersonnel.Models.Employees;


namespace Infrastructures.Storages.EmployeStorages
{
    public class EmployeStorage
	{
		private readonly string _connectionString;

	
		public EmployeStorage(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DBConnection");
		}

		private const string _selectAllQuery = @"
            SELECT E.EmployeID, E.Nom, E.Prenom, E.DateDeNaissance, E.NSecuriteSocial, E.Adresse, E.GroupSanguin, 
                   E.NTelephone, E.FonctionID, E.DateEntree, E.DateSortie, E.SitiationFamiliale, 
                   E.Photo, F.NomFonction
            FROM Employes E
            INNER JOIN Fonctions F ON E.FonctionID = F.FonctionID
            WHERE E.status = 1";
		private const string _selectByIdQuery = @"
            SELECT E.EmployeID, E.Nom, E.Prenom, E.DateDeNaissance, E.NSecuriteSocial, E.Adresse, E.GroupSanguin, 
                   E.NTelephone, E.FonctionID, E.DateEntree, E.DateSortie, E.SitiationFamiliale, 
                   E.Photo, F.NomFonction
            FROM Employes E
            INNER JOIN Fonctions F ON E.FonctionID = F.FonctionID
            WHERE E.EmployeID = @id";
		private const string _insertQuery = "INSERT INTO Employes (Nom, Prenom, DateDeNaissance, NSecuriteSocial, Adresse, GroupSanguin, NTelephone, FonctionID, DateEntree, DateSortie, SitiationFamiliale, Photo) VALUES (@Nom, @Prenom, @DateDeNaissance, @NSecuriteSocial, @Adresse, @GroupSanguin, @NTelephone, @FonctionID, @DateEntree, @DateSortie, @SitiationFamiliale, @Photo); SELECT SCOPE_IDENTITY();";
		private const string _updateQuery = "UPDATE Employes SET Nom = @Nom, Prenom = @Prenom, DateDeNaissance = @DateDeNaissance, NSecuriteSocial = @NSecuriteSocial, Adresse = @Adresse, GroupSanguin = @GroupSanguin, NTelephone = @NTelephone, FonctionID = @FonctionID, DateEntree = @DateEntree, DateSortie = @DateSortie, SitiationFamiliale = @SitiationFamiliale, Photo = @Photo WHERE EmployeID = @EmployeID;";
		private const string _deleteQuery = "UPDATE Employes SET status = 0, DateSortie = @DateSortie WHERE EmployeID = @EmployeID;";

		private const string _selectByFunctionIdQuery = @"
            SELECT E.EmployeID, E.Nom, E.Prenom, E.DateDeNaissance, E.NSecuriteSocial, E.Adresse, E.GroupSanguin, 
                   E.NTelephone, E.FonctionID, E.DateEntree, E.DateSortie, E.SitiationFamiliale, 
                   E.Photo, F.NomFonction
            FROM Employes E
            INNER JOIN Fonctions F ON E.FonctionID = F.FonctionID
            WHERE E.FonctionID = @FonctionID AND E.status = 1";

		private const string _selectEmployeeIdByNameAndFunctionQuery = @"
    SELECT E.EmployeID
    FROM Employes E
    INNER JOIN Fonctions F ON E.FonctionID = F.FonctionID
    WHERE E.Nom = @Nom AND E.Prenom = @Prenom AND F.NomFonction = @NomFonction";




		private static Employe GetEmployeFromDataRow(DataRow row)
		{
            return new Employe
            {
                EmployeID = row["EmployeID"] != DBNull.Value ? (int)row["EmployeID"] : 0,
                Nom = row["Nom"] != DBNull.Value ? (string)row["Nom"] : string.Empty,  // Check for DBNull
                Prenom = row["Prenom"] != DBNull.Value ? (string)row["Prenom"] : string.Empty,  // Check for DBNull
                DateDeNaissance = (DateTime)(row["DateDeNaissance"] != DBNull.Value ? (DateTime)row["DateDeNaissance"] : (DateTime?)null),
                NSecuriteSocial = row["NSecuriteSocial"] != DBNull.Value ? (string)row["NSecuriteSocial"] : string.Empty,  // Check for DBNull
                Adresse = row["Adresse"] != DBNull.Value ? (string)row["Adresse"] : string.Empty,  // Check for DBNull
                GroupSanguin = row["GroupSanguin"] != DBNull.Value ? (string)row["GroupSanguin"] : string.Empty,  // Check for DBNull
                NTelephone = row["NTelephone"] != DBNull.Value ? (string)row["NTelephone"] : string.Empty,  // Check for DBNull
                FonctionID = row["FonctionID"] != DBNull.Value ? Convert.ToInt32(row["FonctionID"]) : 0,
                DateEntree = (DateTime)(row["DateEntree"] != DBNull.Value ? (DateTime)row["DateEntree"] : (DateTime?)null),
                DateSortie = row["DateSortie"] != DBNull.Value ? (DateTime?)row["DateSortie"] : null,
                SitiationFamiliale = row["SitiationFamiliale"] != DBNull.Value ? (string)row["SitiationFamiliale"] : string.Empty,  // Check for DBNull
                Photo = row["Photo"] != DBNull.Value ? (byte[])row["Photo"] : null,
                FonctionName = row["NomFonction"] != DBNull.Value ? (string)row["NomFonction"] : string.Empty  // Check for DBNull
            };
        }

		public async Task<List<Employe>> GetAll()
		{
			await using var connection = new SqlConnection(_connectionString);
			SqlCommand cmd = new(_selectAllQuery, connection);

			DataTable dataTable = new();
			SqlDataAdapter da = new(cmd);

			await connection.OpenAsync();
			da.Fill(dataTable);

			return (from DataRow row in dataTable.Rows select GetEmployeFromDataRow(row)).ToList();
		}

		public async Task<Employe?> GetById(int id)
		{
			await using var connection = new SqlConnection(_connectionString);

			SqlCommand cmd = new(_selectByIdQuery, connection);
			cmd.Parameters.AddWithValue("@id", id);

			DataTable dataTable = new();
			SqlDataAdapter da = new(cmd);

			await connection.OpenAsync();
			da.Fill(dataTable);

			return dataTable.Rows.Count == 0 ? null : GetEmployeFromDataRow(dataTable.Rows[0]);
		}

		public async Task Add(Employe employe)
		{
			await using var connection = new SqlConnection(_connectionString);
			SqlCommand cmd = new(_insertQuery, connection);
			cmd.Parameters.AddWithValue("@Nom", employe.Nom);
			cmd.Parameters.AddWithValue("@Prenom", employe.Prenom);
			cmd.Parameters.AddWithValue("@DateDeNaissance", employe.DateDeNaissance);
			cmd.Parameters.AddWithValue("@NSecuriteSocial", employe.NSecuriteSocial);
			cmd.Parameters.AddWithValue("@Adresse", employe.Adresse);
			cmd.Parameters.AddWithValue("@GroupSanguin", employe.GroupSanguin);
			cmd.Parameters.AddWithValue("@NTelephone", employe.NTelephone);
			cmd.Parameters.AddWithValue("@FonctionID", employe.FonctionID);
			cmd.Parameters.AddWithValue("@DateEntree", employe.DateEntree);
			cmd.Parameters.AddWithValue("@DateSortie", employe.DateSortie ?? (object)DBNull.Value);
			cmd.Parameters.AddWithValue("@SitiationFamiliale", employe.SitiationFamiliale);
			cmd.Parameters.AddWithValue("@Photo", employe.Photo ?? (object)DBNull.Value);

			await connection.OpenAsync();
			var id = await cmd.ExecuteScalarAsync();
			employe.EmployeID = Convert.ToInt32(id);
		}

		public async Task Update(Employe employe)
		{
			await using var connection = new SqlConnection(_connectionString);
			SqlCommand cmd = new(_updateQuery, connection);
			cmd.Parameters.AddWithValue("@Nom", employe.Nom);
			cmd.Parameters.AddWithValue("@Prenom", employe.Prenom);
			cmd.Parameters.AddWithValue("@DateDeNaissance", employe.DateDeNaissance);
			cmd.Parameters.AddWithValue("@NSecuriteSocial", employe.NSecuriteSocial);
			cmd.Parameters.AddWithValue("@Adresse", employe.Adresse);
			cmd.Parameters.AddWithValue("@GroupSanguin", employe.GroupSanguin);
			cmd.Parameters.AddWithValue("@NTelephone", employe.NTelephone);
			cmd.Parameters.AddWithValue("@FonctionID", employe.FonctionID);
			cmd.Parameters.AddWithValue("@DateEntree", employe.DateEntree);
			cmd.Parameters.AddWithValue("@DateSortie", employe.DateSortie ?? (object)DBNull.Value);
			cmd.Parameters.AddWithValue("@SitiationFamiliale", employe.SitiationFamiliale);
			cmd.Parameters.AddWithValue("@Photo", employe.Photo ?? (object)DBNull.Value);
			cmd.Parameters.AddWithValue("@EmployeID", employe.EmployeID);

			await connection.OpenAsync();
			await cmd.ExecuteNonQueryAsync();
		}

		public async Task Delete(int id)
		{
			await using var connection = new SqlConnection(_connectionString);
			SqlCommand cmd = new(_deleteQuery, connection);
			cmd.Parameters.AddWithValue("@EmployeID", id);
			cmd.Parameters.AddWithValue("@DateSortie", DateTime.Now);

			await connection.OpenAsync();
			await cmd.ExecuteNonQueryAsync();
		}

		public async Task<int> GetTotalNumberOfEmployees()
		{
			await using var connection = new SqlConnection(_connectionString);
			SqlCommand cmd = new("CalculerNombreTotalEmployes", connection);
			cmd.CommandType = CommandType.StoredProcedure;

			var param = new SqlParameter("@nombreTotalEmployes", SqlDbType.Int);
			param.Direction = ParameterDirection.Output;
			cmd.Parameters.Add(param);

			await connection.OpenAsync();
			await cmd.ExecuteNonQueryAsync();

			return (int)cmd.Parameters["@nombreTotalEmployes"].Value;
		}

		public async Task<decimal> GetTotalSalaryForMonth(DateTime month)
		{
			try
			{
				await using var connection = new SqlConnection(_connectionString);
				SqlCommand cmd = new("CalculerTotalSalairesDansUnMois", connection);
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@mois", month);

				var param = new SqlParameter("@totalSalaires", SqlDbType.Decimal);
				param.Direction = ParameterDirection.Output;
				param.Precision = 10;
				param.Scale = 2;
				cmd.Parameters.Add(param);

				await connection.OpenAsync();
				await cmd.ExecuteNonQueryAsync();

				// Check if the output parameter value is DBNull or NULL
				if (cmd.Parameters["@totalSalaires"].Value == DBNull.Value || cmd.Parameters["@totalSalaires"].Value == null)
				{
					return 0; // Or handle DBNull/NULL case as per your application logic
				}

				return Convert.ToDecimal(cmd.Parameters["@totalSalaires"].Value);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while fetching the total salary for the month.", ex);
			}
		}

		public async Task<List<Employe>> GetEmployeesByFunctionId(int fonctionId)
		{
			await using var connection = new SqlConnection(_connectionString);
			SqlCommand cmd = new(_selectByFunctionIdQuery, connection);
			cmd.Parameters.AddWithValue("@FonctionID", fonctionId);

			DataTable dataTable = new();
			SqlDataAdapter da = new(cmd);

			await connection.OpenAsync();
			da.Fill(dataTable);

			return (from DataRow row in dataTable.Rows select GetEmployeFromDataRow(row)).ToList();
		}
		public async Task<int?> GetEmployeeIdByName(string nom, string prenom, string nomFonction)
		{
			await using var connection = new SqlConnection(_connectionString);
			SqlCommand cmd = new(_selectEmployeeIdByNameAndFunctionQuery, connection);
			cmd.Parameters.AddWithValue("@Nom", nom);
			cmd.Parameters.AddWithValue("@Prenom", prenom);
			cmd.Parameters.AddWithValue("@NomFonction", nomFonction);

			await connection.OpenAsync();

			var result = await cmd.ExecuteScalarAsync();

			// Check if result is DBNull
			if (result == DBNull.Value || result == null)
			{
				return null; // Employee not found
			}

			return Convert.ToInt32(result); // Return the EmployeeID
		}

	}
}