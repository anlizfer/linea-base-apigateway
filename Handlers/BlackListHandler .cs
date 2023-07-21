using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;

namespace apigateway.microservice.Handlers
{
    public class BlackListHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var myHeader = request.Headers.FirstOrDefault(c => c.Key == "tenant-id");            
            //Si llega el tenant-id continua con la petición
            if (myHeader.Value != null && myHeader.Value.Any())
            {
                //Agregamos el userId a la petición del microservicio
                var stream = request.Headers?.Authorization?.ToString();
                stream = stream?.Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                if (stream != null) {
                    var jsonToken = handler?.ReadToken(stream);
                    var tokenS = jsonToken as JwtSecurityToken;
                    var UserId = tokenS?.Payload["userId"].ToString();
                    request.Headers?.Add("user-id", UserId);
                }

                //Agregamos el correlationId a la petición del microservicio                
                Guid myuuid = Guid.NewGuid();
                string myuuidAsString = myuuid.ToString();
                request.Headers?.Add("correlation-id", myuuidAsString);
                return await base.SendAsync(request, cancellationToken);
            }
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            response.ReasonPhrase = "Your header is not valid";
            return await Task.FromResult<HttpResponseMessage>(response);

        }
    }
}
