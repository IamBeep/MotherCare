using mothercare.Data;
using mothercare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mothercare.Models.Home
{
    public class OrderHistoryClass
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        dbmothercareEntities db = new dbmothercareEntities();
        public List<Tbl_Product> products { get; set; }
        public IEnumerable<Tbl_Cart> cart { get; set; }
        public IEnumerable<Tbl_CartItems> cartItems { get; set; }
        public OrderHistoryClass CreateModel(string name)
        {
            var dataItem = db.Tbl_Members.Where(x => x.EmailId == name).SingleOrDefault();
            var data1 = _unitOfWork.GetRepositoryInstance<Tbl_Cart>().GetAllRecords().OrderByDescending(y => y.Date).Where(x => x.MemberId == dataItem.MemberId);
            var data2 = _unitOfWork.GetRepositoryInstance<Tbl_Product>().GetAllRecords().ToList();
            var data3= _unitOfWork.GetRepositoryInstance<Tbl_CartItems>().GetAllRecords().ToList();
            return new OrderHistoryClass
            {
                cart=data1,
                products=data2,
                cartItems=data3
            };
        }

    }
}