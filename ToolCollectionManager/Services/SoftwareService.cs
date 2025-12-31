using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ToolCollectionManager.Data;
using ToolCollectionManager.Models;

namespace ToolCollectionManager.Services
{
    public class SoftwareService : ISoftwareService
    {
        private readonly AppDbContext _context;

        public SoftwareService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SoftwareItem>> GetAllSoftwareAsync()
        {
            return await _context.SoftwareItems.Include(s => s.Category).ToListAsync();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                // Check if any software is using this category
                bool isUsed = await _context.SoftwareItems.AnyAsync(s => s.CategoryId == id);
                if (isUsed)
                {
                    throw new InvalidOperationException("Cannot delete a category that is in use by software items.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddSoftwareAsync(SoftwareItem software)
        {
            _context.SoftwareItems.Add(software);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSoftwareAsync(SoftwareItem software)
        {
            _context.SoftwareItems.Update(software);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSoftwareAsync(int id)
        {
            var item = await _context.SoftwareItems.FindAsync(id);
            if (item != null)
            {
                _context.SoftwareItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<SoftwareItem>> SearchSoftwareAsync(string query, int? categoryId)
        {
            var dbQuery = _context.SoftwareItems.Include(s => s.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                dbQuery = dbQuery.Where(s => s.Name.Contains(query) || s.Description.Contains(query));
            }

            if (categoryId.HasValue)
            {
                dbQuery = dbQuery.Where(s => s.CategoryId == categoryId.Value);
            }

            return await dbQuery.ToListAsync();
        }

        public async Task LaunchSoftwareAsync(string executablePath)
        {
            if (string.IsNullOrWhiteSpace(executablePath)) return;
            
            try
            {
                await Task.Run(() =>
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = executablePath,
                        UseShellExecute = true
                    });
                });
            }
            catch (Exception)
            {
                // In a real app, we should log this
                throw; 
            }
        }
    }
}