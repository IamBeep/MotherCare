using mothercare.Data;
using mothercare.Repository;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mothercare.Models.Home
{
    public class productViewClass
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        dbmothercareEntities context = new dbmothercareEntities();
        public Tbl_Product products { get; set; }
        public List<Tbl_comment> comments { get; set; }
        public List<Tbl_comment> ratings { get; set; }
        public int cartCheck = 0;
        public int data4 = 0;
        public int data6 = 0;
        public int already;
        public double rating = 0, rating2=0, counter=0, rater=0;
        public productViewClass CreateModel(int productId, string session)
        {
            var data2 = context.Tbl_Product.Where(x => x.ProductId==productId).SingleOrDefault();
            List<Tbl_comment> data3 = _unitOfWork.GetRepositoryInstance<Tbl_comment>().GetAllRecords().OrderByDescending(y=>y.Date).Where(x => x.ProductId == productId).ToList();
            if(session!="null")
            {
                data4 = _unitOfWork.GetRepositoryInstance<Tbl_CartItems>().GetAllRecords().Where(x => x.ProductId==productId && x.Tbl_Cart.Tbl_Members.EmailId==session).ToList().Count();
                data6 = _unitOfWork.GetRepositoryInstance<Tbl_comment>().GetAllRecords().Where(x => x.ProductId == productId && x.Tbl_Members.EmailId == session && x.Rating!=null).ToList().Count();
            }
            List<Tbl_comment> data5 = _unitOfWork.GetRepositoryInstance<Tbl_comment>().GetAllRecords().Where(x => x.Rating!=null && x.ProductId==productId).ToList();
            if(data5!=null)
            {
                foreach (var rate in data5)
                {
                    counter++;
                    rating += Convert.ToDouble(rate.Rating);
                }
                if(rating!=0 && counter!=0)
                {
                    rating = rating / counter;
                }
                else
                {
                    rating = 0;
                }
                rating=Math.Round(rating, 2);
            }
            return new productViewClass
            {
                products = data2,
                comments = data3,
                cartCheck = data4,
                ratings = data5,
                already=data6,
                rating2=rating,
                rater=counter
            };
        }

    }
}