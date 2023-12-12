using System;
using System.Text;

namespace HereticalSolutions
{
	/// <summary>
	/// Contains extension methods for manipulating addresses.
	/// </summary>
	public static class AddressExtensions
	{
		private const char ADDRESS_SEPARATOR = '/';

		public static string[] SplitAddressBySeparator(this string address)
		{
			if (string.IsNullOrEmpty(address))
				return new string[0];

			string[] localAddresses = address.Split(ADDRESS_SEPARATOR);

			return localAddresses;
		}

		/// <summary>
		/// Converts the address to its hash code.
		/// </summary>
		/// <param name="address">The address to convert.</param>
		/// <returns>The hash code of the address.</returns>
		public static int AddressToHash(this string address)
		{
			return address.GetHashCode();
		}

		/// <summary>
		/// Converts each part of the address to its hash code.
		/// </summary>
		/// <param name="address">The address to convert.</param>
		/// <returns>An array of hash codes representing each part of the address.</returns>
		public static int[] AddressToHashes(this string address)
		{
			if (string.IsNullOrEmpty(address))
				return new int[0];

			string[] localAddresses = address.Split(ADDRESS_SEPARATOR);

			int[] result = new int[localAddresses.Length];

			for (int i = 0; i < result.Length; i++)
				result[i] = localAddresses[i].GetHashCode();

			return result;
		}

		/// <summary>
		/// Returns a subset of the address hashes up to the specified depth.
		/// </summary>
		/// <param name="addressHashes">The array of address hashes.</param>
		/// <param name="depth">The depth of the subset.</param>
		/// <returns>A subset of the address hashes up to the specified depth.</returns>
		public static int[] PartialAddressHashes(this int[] addressHashes, int depth)
		{
			int effectiveDepth = Math.Min(addressHashes.Length, depth);

			int[] result = new int[effectiveDepth];

			for (int i = 0; i < result.Length; i++)
				result[i] = addressHashes[i];

			return result;
		}

		/// <summary>
		/// Returns the address part at the specified depth.
		/// </summary>
		/// <param name="addressParts">The array of address parts.</param>
		/// <param name="depth">The depth of the address part.</param>
		/// <returns>The address part at the specified depth.</returns>
		public static string AddressAtDepth(this string[] addressParts, int depth)
		{
			return addressParts[depth];
		}

		/// <summary>
		/// Returns a partial address up to the specified depth.
		/// </summary>
		/// <param name="addressParts">The array of address parts.</param>
		/// <param name="depth">The depth of the partial address.</param>
		/// <returns>A partial address up to the specified depth.</returns>
		public static string PartialAddress(this string[] addressParts, int depth)
		{
			int effectiveDepth = Math.Min(addressParts.Length, depth);

			StringBuilder stringBuilder = new StringBuilder();

			for (int i = 0; i < effectiveDepth; i++)
			{
				stringBuilder.Append(addressParts[i]);

				if (i + 1 < effectiveDepth)
					stringBuilder.Append(ADDRESS_SEPARATOR);
			}

			return stringBuilder.ToString();
		}

		public static string ReplaceLast(this string address, string postfix)
		{
			if (string.IsNullOrEmpty(address))
				return string.Empty;

			string[] parts = address.Split(ADDRESS_SEPARATOR);

			parts[parts.Length - 1] = postfix;

			return string.Join(ADDRESS_SEPARATOR.ToString(), parts);
		}
	}
}