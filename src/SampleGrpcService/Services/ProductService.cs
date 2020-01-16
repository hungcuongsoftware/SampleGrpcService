using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace SampleGrpcService
{
    public class ProductService : ProductApi.ProductApiBase
    {
        private readonly ILogger<ProductService> _logger;

        public List<Product> ProductList { get; set; }

        public ProductService(ILogger<ProductService> logger)
        {
            _logger = logger;
            ProductList = new List<Product>
            {
                new Product {ProductId=1, ProductName="Product Name 1", Price =100 },
                new Product {ProductId=2, ProductName="Product Name 2", Price =200 },
                new Product {ProductId=3, ProductName="Product Name 3", Price =300 },
                new Product {ProductId=4, ProductName="Product Name 4", Price =400 },
                new Product {ProductId=5, ProductName="Product Name 5", Price =500 },
            };
        }

        /// <summary>
        /// Type 1: Unary Call
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<GetPrductByIdResponse> GetProductById(GetProductByIdRequest request, ServerCallContext context)
        {
            var product = ProductList.SingleOrDefault(o=>o.ProductId==request.ProductId);
            var response = new GetPrductByIdResponse();
            if (product != null)
            {
                context.Status = new Status(StatusCode.OK, "Product was found");
                response.Product = product;
            }
            else
            {
                context.Status = new Status(StatusCode.NotFound, "Product was not found");
            }
            
            return Task.FromResult(response);
        }

        /// <summary>
        /// Type 2: Server Streaming, return each stream is an product
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task GetProductListing(GetProductListingRequest request, IServerStreamWriter<Product> responseStream, ServerCallContext context)
        {
            foreach (var product in ProductList)
            {
                await responseStream.WriteAsync(product);
            }
        }

        /// <summary>
        /// Type 3: Client Streaming, client send to client each product on each stream
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<BulkInsertProductResponse> BulkInsertProduct(IAsyncStreamReader<Product> requestStream, ServerCallContext context)
        {
            var products = new List<Product>();
            while (await requestStream.MoveNext())
            {
                var product = requestStream.Current;
                if (product != null)
                {
                    products.Add(product);
                }
            }

            return new BulkInsertProductResponse
            {
                TotalRecords = products.Count
            };
        }

        public async override Task BulkUpdateProduct(IAsyncStreamReader<Product> requestStream, IServerStreamWriter<Product> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var product = requestStream.Current;
                if (product != null)
                {
                    product.Price += 100;
                    await responseStream.WriteAsync(product);
                }
            }
        }
    }
}
