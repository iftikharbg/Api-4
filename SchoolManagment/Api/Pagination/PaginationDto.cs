using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Pagination
{
    public class PaginationDto<T>
    {
        public PaginationDto(IQueryable<T> items,int pageNumber,int itemCount)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (itemCount < 1) itemCount = 10;


            this.Items = Items.Skip((pageNumber - 1) * itemCount).Take(itemCount).ToList();
            this.TotalPages = (int)Math.Ceiling((decimal)items.Count() / itemCount);
            this.CurrentPage = pageNumber;
            this.HasNext = CurrentPage < TotalPages;
            this.HasPrevious = CurrentPage > 1;

        
            
        }

        public List<T> Items { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public bool HasNext { get; set; }

        public bool HasPrevious { get; set; }
    }
}
