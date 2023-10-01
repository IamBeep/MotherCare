using mothercare.Data;
using mothercare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mothercare.Models.Home
{
    public class FilterClass
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        dbmothercareEntities context = new dbmothercareEntities();
        public List<Tbl_Product> products { get; set; }
        public List<Tbl_Category> categories { get; set; }
        public int results=0,count = 0;
        public FilterClass CreateModel(string name, int category)
        {
            List<Tbl_Product> data1=new List<Tbl_Product>();
            List<Tbl_Category> data2 = context.Tbl_Category.ToList();
            if (name == "category")
            {
                data1 = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetAllRecords().Where(x => x.CategoryId==category).ToList();
                count = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetAllRecords().Where(x => x.CategoryId == category).ToList().Count();
            }
            else if (name == "rating")
            {
                data1 = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetAllRecords().Where(x => Convert.ToDouble(x.rating)>=category-1 && Convert.ToDouble(x.rating)<=category).ToList();
                count = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetAllRecords().Where(x => Convert.ToDouble(x.rating)>=category-1 && Convert.ToDouble(x.rating)<=category).ToList().Count();
            }
            if (data1 != null)
            {
                return new FilterClass
                {
                    products = data1,
                    categories=data2,
                    results=count
                };
            }
            else
            {
                return new FilterClass
                {
                    products = null,
                    categories = data2,
                    results = count
                };
            }
            
        }
    }
}