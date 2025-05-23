﻿namespace Controle_Pessoal.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
        public string? GoogleId { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}
