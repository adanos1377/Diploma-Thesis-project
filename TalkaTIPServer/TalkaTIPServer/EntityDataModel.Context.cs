﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TalkaTIPServer
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TalkaTipDB : DbContext
    {
        public TalkaTipDB()
            : base("name=TalkaTipDB")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Blocked> Blocked { get; set; }
        public virtual DbSet<Friends> Friends { get; set; }
        public virtual DbSet<GroupChat> GroupChat { get; set; }
        public virtual DbSet<GroupChatMessages> GroupChatMessages { get; set; }
        public virtual DbSet<GroupChatUsers> GroupChatUsers { get; set; }
        public virtual DbSet<Histories> Histories { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersAPIs> UsersAPIs { get; set; }
    }
}