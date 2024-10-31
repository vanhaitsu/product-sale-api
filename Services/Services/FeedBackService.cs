using Repositories.Entities;
using Repositories.Interfaces;
using Repositories.Models.CartItemModels;
using Services.Common;
using Services.Interfaces;
using Services.Models.CartItemModels;
using Services.Models.FeedBackModels;
using Services.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class FeedBackService : IFeedBackService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeedBackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseModel> Create(FeedBackCreateModel feedBackCreateModel)
        {
            var checkBoughtProduct = await _unitOfWork.OrderRepository.CheckBuyProductAlready(feedBackCreateModel.AccountID, feedBackCreateModel.ProductID);
            if(checkBoughtProduct == false)
            {
                return new ResponseModel { Message = "You must to buy this product to provide feedback!", Status = false };

            }
            var checkFeedBack = await _unitOfWork.FeedBackRepository.CheckFeedBackAlready(feedBackCreateModel.AccountID, feedBackCreateModel.ProductID);
            if (checkFeedBack != null)
            {
                return new ResponseModel { Message = "Your account has already provided feedback on this product!", Status = false };
            }
            if(feedBackCreateModel.Rating > 5 || feedBackCreateModel.Rating < 1 || feedBackCreateModel.Rating == null)
            {
                return new ResponseModel { Message = "Not valid rating!", Status = false };

            }
            var newFeedBack = new FeedBack
            {
                Id = Guid.NewGuid(),
                AccountID = feedBackCreateModel.AccountID,
                ProductID = feedBackCreateModel.ProductID,
                Rating = feedBackCreateModel.Rating,
                Description = feedBackCreateModel.Description
            };
            await _unitOfWork.FeedBackRepository.AddAsync(newFeedBack);
            var result = await _unitOfWork.SaveChangeAsync();
            return result > 0
                    ? new ResponseModel { Message = "Feedback submitted successfully!", Status = true }
                    : new ResponseModel { Message = "Failed to submit feedback.", Status = false };
        }

        public async Task<Pagination<GetFeedBacksModel>> GetFeedBacks(FeedBackFilterModel feedBackFilterModel)
        {
            var cart = await _unitOfWork.FeedBackRepository.GetFeedBacksByProductId(feedBackFilterModel.ProductID);

            if (cart == null)
            {
                return new Pagination<GetFeedBacksModel>(
                new List<GetFeedBacksModel>(),
                           feedBackFilterModel.PageIndex,
                           feedBackFilterModel.PageSize, 0);
            }

            var feedBacksResult = await _unitOfWork.FeedBackRepository.GetAllAsync(
                filter: _ => _.ProductID == feedBackFilterModel.ProductID,
                include: "Account, Product",
                pageIndex: feedBackFilterModel.PageIndex,
                pageSize: feedBackFilterModel.PageSize
            );

            if (feedBacksResult != null || feedBacksResult.Data.Any())
            {
                var feedBackModelList = feedBacksResult.Data.Select(_ => new GetFeedBacksModel
                {
                    Id = _.Id,
                    ProductID = _.ProductID,    
                    AccountID = _.AccountID,
                    AccountName = _.Account.FirstName + " " + _.Account.LastName,
                    Description = _.Description,
                    Rating = _.Rating
                }).ToList();

                return new Pagination<GetFeedBacksModel>(
                feedBackModelList,
                    feedBackFilterModel.PageIndex,
                    feedBackFilterModel.PageSize,
                    feedBacksResult.TotalCount
                );
            }
            return null;
        }
    }
}
