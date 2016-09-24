using System;
using Gis.Crypto;
using Gis.Infrastructure.NsiCommonService;

namespace Gis
{
	class Program
	{
		static void Main(string[] args)
		{
			var service = new NsiPortsTypeClient();
			service.ClientCredentials.UserName.UserName = "lanit";
			service.ClientCredentials.UserName.Password = "tv,n8!Ya";

			var request = new exportNsiListRequest1
			{
				ISRequestHeader = new HeaderType
				{
					Date = DateTime.Now,
					MessageGUID = Guid.NewGuid().ToString()
				},
				exportNsiListRequest = new exportNsiListRequest
				{
					version = "10.0.1.2",
					ListGroup = ListGroup.NSI,
					ListGroupSpecified = true,
					Id = CryptoConsts.CONTAINER_ID
				}
			};

			var result = service.exportNsiList(request);
		}
	}
}