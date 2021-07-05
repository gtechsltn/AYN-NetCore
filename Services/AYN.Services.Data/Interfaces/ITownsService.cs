﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace AYN.Services.Data.Interfaces
{
    public interface ITownsService
    {
        Task<IEnumerable<KeyValuePair<string, string>>> GetAllAsKeyValuePairsAsync();

        int GetIdByName(string townName);

        bool IsExisting(int townId);

        bool IsTownContainsGivenAddress(int townId, int addressId);
    }
}
