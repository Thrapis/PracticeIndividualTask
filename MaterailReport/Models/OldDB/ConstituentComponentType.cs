using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaterailReport.Models.OldDB
{
    [Table("CONSTITUENT_COMPONENT_TYPE")]
    public class ConstituentComponentType
    {
        public int Id { get; set; } 
        public string Name { get; set; } 
        public bool Recyclable { get; set; } 
        public string Description { get; set; } 
    }

    public class ConstituentComponentTypeRepository : IGenericRepositoryIntPK<ConstituentComponentType>
    {
        DbContext _context;
        DbSet<ConstituentComponentType> _dbSet;

        public ConstituentComponentTypeRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<ConstituentComponentType>();
        }

        public ConstituentComponentTypeRepository()
        {
            _context = new MRContext();
            _dbSet = _context.Set<ConstituentComponentType>();
        }

        public IEnumerable<ConstituentComponentType> Get()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public IEnumerable<ConstituentComponentType> Get(Func<ConstituentComponentType, bool> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate).ToList();
        }

        public ConstituentComponentType FindById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Create(ConstituentComponentType item)
        {
            _dbSet.Add(item);
            _context.SaveChanges();
        }

        public void Update(ConstituentComponentType item)
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Remove(ConstituentComponentType item)
        {
            _dbSet.Remove(item);
            _context.SaveChanges();
        }
    }

}