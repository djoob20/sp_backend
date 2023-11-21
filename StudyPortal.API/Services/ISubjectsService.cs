using System;
using StudyPortal.API.Models;

namespace StudyPortal.API.Services
{
    public interface ISubjectsService<T> where T : Subjects
    {
        public Task<List<T>> GetAsync();

        public Task<T?> GetAsync(string id);

        public Task CreateAsync(T newSubject);

        public Task UpdateAsync(string id, T updatedSubject);

        public Task DeleteAsync(string id);
    }
}

