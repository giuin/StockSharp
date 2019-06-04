namespace StockSharp.Messages
{
	using System;
	using System.Collections.Generic;

	using Ecng.Collections;

	/// <summary>
	/// The portfolio based message adapter's provider interface. 
	/// </summary>
	public interface IPortfolioMessageAdapterProvider
	{
		/// <summary>
		/// All available adapters.
		/// </summary>
		IEnumerable<KeyValuePair<string, IMessageAdapter>> Adapters { get; }

		/// <summary>
		/// Association changed.
		/// </summary>
		event Action Changed;

		/// <summary>
		/// Get adapter by portfolio name.
		/// </summary>
		/// <param name="portfolioName">Portfolio name.</param>
		/// <returns>The found adapter.</returns>
		IMessageAdapter GetAdapter(string portfolioName);

		/// <summary>
		/// Make association adapter and portfolio name.
		/// </summary>
		/// <param name="portfolioName">Portfolio name.</param>
		/// <param name="adapter">The adapter.</param>
		void SetAdapter(string portfolioName, IMessageAdapter adapter);

		/// <summary>
		/// Remove association between portfolio name and adapter.
		/// </summary>
		/// <param name="portfolioName">Portfolio name.</param>
		/// <returns><see langword="true"/> if the association is successfully removed, otherwise, <see langword="false"/>.</returns>
		bool RemoveAssociation(string portfolioName);
	}

	/// <summary>
	/// In memory implementation of <see cref="IPortfolioMessageAdapterProvider"/>.
	/// </summary>
	public class InMemoryPortfolioMessageAdapterProvider : IPortfolioMessageAdapterProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InMemoryPortfolioMessageAdapterProvider"/>.
		/// </summary>
		public InMemoryPortfolioMessageAdapterProvider()
		{
		}

		private readonly CachedSynchronizedDictionary<string, IMessageAdapter> _adapters = new CachedSynchronizedDictionary<string, IMessageAdapter>(StringComparer.InvariantCultureIgnoreCase);

		/// <inheritdoc />
		public virtual IEnumerable<KeyValuePair<string, IMessageAdapter>> Adapters => _adapters.CachedPairs;

		/// <inheritdoc />
		public event Action Changed;

		/// <inheritdoc />
		public virtual IMessageAdapter GetAdapter(string portfolioName)
		{
			if (portfolioName.IsEmpty())
				throw new ArgumentNullException(nameof(portfolioName));

			return _adapters.TryGetValue(portfolioName);
		}

		/// <inheritdoc />
		public virtual void SetAdapter(string portfolioName, IMessageAdapter adapter)
		{
			if (portfolioName.IsEmpty())
				throw new ArgumentNullException(nameof(portfolioName));

			IMessageAdapter prev;

			lock (_adapters.SyncRoot)
			{
				prev = _adapters.TryGetValue(portfolioName);
				_adapters[portfolioName] = adapter;
			}

			if (prev != adapter)
				Changed?.Invoke();
		}

		/// <inheritdoc />
		public virtual bool RemoveAssociation(string portfolioName)
		{
			if (portfolioName.IsEmpty())
				throw new ArgumentNullException(nameof(portfolioName));

			if (!_adapters.Remove(portfolioName))
				return false;

			Changed?.Invoke();
			return true;
		}
	}
}