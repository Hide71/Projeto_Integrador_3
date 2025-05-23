﻿using System.ComponentModel.DataAnnotations;

namespace Controle_Pessoal.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public int UserId { get; set;}               
        public User User { get; set; }

        public int CategoryId { get; set;}               
        public Category Category { get; set; }
        
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }
    }
}
