﻿namespace Entities.DataTransferObject
{

    public record BookDto
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public decimal Price { get; set; }

    }


}
