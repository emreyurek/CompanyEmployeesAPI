using System;

namespace Entitites.Exceptions
{
    public sealed class CompanyCollectionBadRequest : BadRequestException
    {
        public CompanyCollectionBadRequest() : base("Company collection sent from a client is null.")
        {
        }
    }
}
