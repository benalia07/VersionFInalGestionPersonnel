using GestionPersonnel.Models.Dettes;

using GestionPersonnel.Models.Salaires;
using GestionPersonnel.Storages.DettesStorages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionPersonnel.Services
{
    public class DetteRestantService : IDetteRestantService
    {
        private readonly DetteRestantStorage _detteRestantStorage;

        public DetteRestantService(DetteRestantStorage detteRestantStorage)
        {
            _detteRestantStorage = detteRestantStorage;
        }

        public async Task<bool> ExisteDettePourEmployeAsync(int employeId)
        {
            return await _detteRestantStorage.ExisteDettePourEmploye(employeId);
        }

        public async Task<List<DetteRestant>> GetDettesRestantesParEmployeAsync(int employeId)
        {
            return await _detteRestantStorage.GetByEmployeIdAsync(employeId);
        }

        public async Task<List<DetteRestant>> GetToutesDettesRestantesAsync()
        {
            return await _detteRestantStorage.GetAll();
        }

        public async Task<DetteRestant?> GetDetteRestanteParIdAsync(int id)
        {
            return await _detteRestantStorage.GetById(id);
        }

        public async Task AjouterDetteRestanteAsync(DetteRestant detteRestant)
        {
            await _detteRestantStorage.Add(detteRestant);
        }

        public async Task ModifierDetteRestanteAsync(DetteRestant detteRestant)
        {
            await _detteRestantStorage.Update(detteRestant);
        }

        public async Task SupprimerDetteRestanteAsync(int id)
        {
            await _detteRestantStorage.Delete(id);
        }
        public async Task MontantRetirer(int employeid, decimal montant)
        {
            await _detteRestantStorage.MontantRetirer(employeid, montant);
        }
    }
}
