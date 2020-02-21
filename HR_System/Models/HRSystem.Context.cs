﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HR_System.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class HRSystemEntities : DbContext
    {
        public HRSystemEntities()
            : base("name=HRSystemEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<t_config> t_config { get; set; }
        public virtual DbSet<t_empleados> t_empleados { get; set; }
        public virtual DbSet<t_objetidet> t_objetidet { get; set; }
        public virtual DbSet<t_objetivos> t_objetivos { get; set; }
        public virtual DbSet<t_plantas> t_plantas { get; set; }
        public virtual DbSet<t_usuarios> t_usuarios { get; set; }
        public virtual DbSet<t_config_m1> t_config_m1 { get; set; }
        public virtual DbSet<t_budget_pta> t_budget_pta { get; set; }
        public virtual DbSet<t_califica> t_califica { get; set; }
        public virtual DbSet<t_config_m4> t_config_m4 { get; set; }
        public virtual DbSet<t_jobcode> t_jobcode { get; set; }
        public virtual DbSet<t_meridet> t_meridet { get; set; }
        public virtual DbSet<t_merit> t_merit { get; set; }
    
        public virtual ObjectResult<sp_view_merit_year_Result> sp_view_merit_year(Nullable<decimal> year)
        {
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_view_merit_year_Result>("sp_view_merit_year", yearParameter);
        }
    
        public virtual ObjectResult<sp_view_merit_dreport_Result> sp_view_merit_dreport(string supervisor)
        {
            var supervisorParameter = supervisor != null ?
                new ObjectParameter("Supervisor", supervisor) :
                new ObjectParameter("Supervisor", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_view_merit_dreport_Result>("sp_view_merit_dreport", supervisorParameter);
        }
    
        public virtual ObjectResult<sp_view_merit_planners_Result> sp_view_merit_planners()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_view_merit_planners_Result>("sp_view_merit_planners");
        }
    }
}
