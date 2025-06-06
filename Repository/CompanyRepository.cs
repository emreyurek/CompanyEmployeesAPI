using Contracts;
using Entitites.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)
        {
            var companies = await FindAll(trackChanges)
            .OrderBy(c => c.Name)
            .ToListAsync();

            return companies;
        }
        public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges) =>
         await FindByCondition(c => c.Id.Equals(companyId), trackChanges)
         .SingleOrDefaultAsync();
        public void CreatCompany(Company company) => Create(company);
        public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
                  await FindByCondition(c => ids.Contains(c.Id), trackChanges)
                  .ToListAsync();
        public void DeleteCompany(Company company) => Delete(company);
    }
}
