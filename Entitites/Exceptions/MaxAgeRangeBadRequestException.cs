using System;

namespace Entitites.Exceptions
{
    public class MaxAgeRangeBadRequestException : BadRequestException
    {
        public MaxAgeRangeBadRequestException() : base("Max age can't be less than min age.")
        {

        }
    }
}
