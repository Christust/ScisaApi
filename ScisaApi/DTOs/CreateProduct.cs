﻿namespace ScisaApi.DTOs
{
    public class CreateProduct
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();

    }
}
