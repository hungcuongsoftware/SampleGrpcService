# SampleGrpcService
- NET CORE 3.1
- gRPC service template on Visual Studio 2019

# gRPC services
The sameplae have 4 method types of gRPC service, which have been defined on proto file
- Unary call: 
  rpc GetProductById (GetProductByIdRequest) returns (GetPrductByIdResponse);

- Server streaming call: 
  rpc GetProductListing (GetProductListingRequest) returns (stream Product);
  
- Client streaming call: 
  rpc BulkInsertProduct (stream Product) returns (BulkInsertProductResponse);
  
- Duplex streaming call: 
  rpc BulkUpdateProduct (stream Product) returns (stream Product);

# gRPC testing tool
- Use BloomRPC to test the sample services
  https://github.com/uw-labs/bloomrpc
