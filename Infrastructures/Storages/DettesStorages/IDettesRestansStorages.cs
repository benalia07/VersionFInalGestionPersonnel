using Infrastructures.Domains.Models.Dettes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Storages.DettesStorages
{
    public interface IDetteRestantStorage
    {
        Task<bool> ExisteDettePourEmploye(int employeId);
        Task<List<DettesRestants>> GetByEmployeIdAsync(int employeId);
        Task<List<DettesRestants>> GetAll();
        Task<DettesRestants?> GetById(int id);
        Task Add(DettesRestants detteRestant);
        Task Update(DettesRestants detteRestant);
        Task Delete(int id);
    }
}
