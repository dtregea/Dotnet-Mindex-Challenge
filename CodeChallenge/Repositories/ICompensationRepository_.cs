using System;
using System.Threading.Tasks;
using CodeChallenge.Models;

namespace ICompensationRepository_
{
    public interface ICompensationRepository
    {
        Compensation GetById(String id);
        Compensation Add(Compensation employee);
        Task SaveAsync();
    }
}