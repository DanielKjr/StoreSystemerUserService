using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Interfaces
{
	/// <summary>
	/// Contains a single method for adding items to a user, since the generic repository doesn't have a generic method for this
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	public interface IRepositoryPatch<TContext> where TContext : DbContext
	{
		/// <summary>
		/// <para>
		/// Adds the list of items to the user. 
		/// </para>
		/// 
		/// <example>
		/// <![CDATA[_repositoryPatch.AddItemsToUser<User, Item>(q => q.Where(x => x.Id == id).Include(i => i.Inventory).Include(i=> i.Inventory.Items), stuff)]]>
		/// </example>
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TCollection"></typeparam>
		/// <param name="queryOperation"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		Task AddItemsToUser<TEntity, TCollection>(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryOperation, TCollection entity, List<TCollection> list) where TEntity : User where TCollection : Item;



	}
}
