using Microsoft.EntityFrameworkCore;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Repository
{
	public class RepositoryPatch<TContext>(IDbContextFactory<TContext> _dbContextFactory) : IRepositoryPatch<TContext> where TContext : DbContext
	{

		//<inheritdoc//
		public async Task AddItemsToUser<TEntity, TCollection>(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryOperation, TCollection entity = null, List<TCollection> list = null) where TEntity : User where TCollection : Item
		{
			await using var context = _dbContextFactory.CreateDbContext();

			var user = queryOperation(context.Set<TEntity>()).FirstOrDefault()!;
			if (user != null)
			{

				context.Entry(user).State = EntityState.Modified;
				foreach (var navigation in context.Entry(user).Navigations)
				{
					if (navigation.Metadata.Name == nameof(User.Inventory) && navigation.CurrentValue is Inventory inventory)
					{
						context.Entry(inventory).State = EntityState.Modified;
						if (inventory.Items != null)
						{
							if (list != null)
							{
								foreach (var item in list)
								{
									if (!inventory.Items.Contains(item))
										user.Inventory.Items.Add(item);
								}
							}
							else
							{
								if (inventory.Items.Count() != 0 && inventory.Items.Find(c => c.Id == entity.Id) != null)
								{
									var s = inventory.Items.Find(c => c.Id == entity.Id);
									s = entity;
									context.Entry(inventory).State = EntityState.Modified;
									context.Entry(entity).State = EntityState.Modified;
								}
								else
								{

									user.Inventory.Items.Add(entity);
									context.Entry(inventory).State = EntityState.Modified;
								}
							}

						}
					}
				}
				await context.SaveChangesAsync();
			}
		}


	}
}
