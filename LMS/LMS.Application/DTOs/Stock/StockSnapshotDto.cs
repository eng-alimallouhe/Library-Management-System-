﻿namespace LMS.Application.DTOs.Stock
{
    public class StockSnapshotDto
    {
        public Guid ProductId { get; set; }      
        public string ProductName { get; set; }  
        public int ProductStock { get; set; }        
        public decimal ProductPrice { get; set; }   
        public decimal TotalValue => ProductPrice * ProductStock;  
        public string UpdatedAt { get; set; }
        public int LogsCount { get; set; }
    }
}