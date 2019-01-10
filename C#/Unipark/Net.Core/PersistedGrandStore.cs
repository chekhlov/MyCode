using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityOAuth2.Server
{
	public class PersistedGrandStore : IPersistedGrantStore
	{

		private readonly ConcurrentDictionary<string, PersistedGrant> _repository = new ConcurrentDictionary<string, PersistedGrant>();

		/// <summary>
		/// Stores the grant.
		/// </summary>
		/// <param name="grant">The grant.</param>
		/// <returns></returns>
		public Task StoreAsync(PersistedGrant grant)
		{
			_repository[grant.Key] = grant;

			return Task.CompletedTask;
		}

		/// <summary>
		/// Gets the grant.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public Task<PersistedGrant> GetAsync(string key)
		{
			PersistedGrant token;
			if (_repository.TryGetValue(key, out token))
			{
				return Task.FromResult(token);
			}

			return Task.FromResult<PersistedGrant>(null);
		}

		/// <summary>
		/// Gets all grants for a given subject id.
		/// </summary>
		/// <param name="subjectId">The subject identifier.</param>
		/// <returns></returns>
		public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
		{
			return Task.FromResult(_repository.Where(x => x.Value.SubjectId == subjectId).Select(x => x.Value).AsEnumerable());
		}

		/// <summary>
		/// Removes the grant by key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public Task RemoveAsync(string key)
		{
			_repository.TryRemove(key, out _);

			return Task.CompletedTask;
		}

		/// <summary>
		/// Removes all grants for a given subject id and client id combination.
		/// </summary>
		/// <param name="subjectId">The subject identifier.</param>
		/// <param name="clientId">The client identifier.</param>
		/// <returns></returns>
		public Task RemoveAllAsync(string subjectId, string clientId)
		{
			var keys = _repository.Where(x => x.Value.ClientId == clientId && x.Value.SubjectId == subjectId).Select(x => x.Key).ToArray();

			foreach (var key in keys)
			{
				_repository.TryRemove(key, out _);
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Removes all grants of a give type for a given subject id and client id combination.
		/// </summary>
		/// <param name="subjectId">The subject identifier.</param>
		/// <param name="clientId">The client identifier.</param>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public Task RemoveAllAsync(string subjectId, string clientId, string type)
		{
			var keys = _repository.Where(x => x.Value.ClientId == clientId && x.Value.SubjectId == subjectId
							&& x.Value.Type == type).Select(x => x.Key).ToArray();

			foreach (var key in keys)
			{
				_repository.TryRemove(key, out _);
			}

			return Task.CompletedTask;
		}
	}
}
