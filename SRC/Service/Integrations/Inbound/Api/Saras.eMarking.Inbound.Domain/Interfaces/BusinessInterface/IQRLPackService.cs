using Saras.eMarking.Inbound.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Inbound.Interfaces.BusinessInterface
{
    public interface IQRLPackService
    {
        Task<List<eMarkingSyncUserResponse>> eMarkingQRLpackStatics();

    }
}
