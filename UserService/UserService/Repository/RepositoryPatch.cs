using Microsoft.EntityFrameworkCore;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Repository
{
	public class RepositoryPatch<TContext>(IDbContextFactory<TContext> _dbContextFactory) : IRepositoryPatch<TContext> where TContext : DbContext
	{

		//<inheritdoc//
		public async Task AddItemsToUser<TEntity, TCollection>(
	Func<IQueryable<TEntity>, IQueryable<TEntity>> queryOperation,
	TCollection entity = null!,
	List<TCollection> list = null!)
	where TEntity : User
	where TCollection : Item
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
						// Ensure the inventory is tracked
						context.Entry(inventory).State = EntityState.Modified;

						if (inventory.Items != null)
						{
							// Handle the list of items being added or updated
							if (list != null)
							{
								foreach (var item in list)
								{
									var existingItem = inventory.Items.FirstOrDefault(i => i.Id == item.Id);
									if (existingItem == null)
									{
										// Add the new item to the inventory
										user.Inventory.Items.Add(item);
										context.Entry(item).State = EntityState.Added;
									}
									else
									{
										// Update the existing item (copy over properties)
										context.Entry(existingItem).CurrentValues.SetValues(item);
										context.Entry(existingItem).State = EntityState.Modified;
									}
								}
							}
							// Handle a single item being added or updated
							else if (entity != null)
							{
								var existingItem = inventory.Items.FirstOrDefault(i => i.Id == entity.Id);
								if (existingItem != null)
								{
									// Update the existing item's properties
									context.Entry(existingItem).CurrentValues.SetValues(entity);
									context.Entry(existingItem).State = EntityState.Modified;
								}
								else
								{
									// Add the new single item to the inventory
									user.Inventory.Items.Add(entity);
									context.Entry(entity).State = EntityState.Added;
								}
							}
						}
					}
				}

				await context.SaveChangesAsync();
			}
		}

		public async Task<bool> UpdateUser(User user)
		{
			await using var context = _dbContextFactory.CreateDbContext();


			// Attach the user entity
			context.Attach(user);

			// Mark user as modified to ensure changes to the user are tracked
			context.Entry(user).State = EntityState.Modified;

			// Ensure the inventory is tracked and set as modified
			if (user.Inventory != null)
			{
				context.Attach(user.Inventory);
				context.Entry(user.Inventory).State = EntityState.Modified;

				// Ensure the items in the inventory are tracked and marked as modified
				foreach (var item in user.Inventory.Items)
				{
					var trackedItem = context.Entry(item);
					if (trackedItem.State == EntityState.Detached)
					{
						// Attach the item if it's not being tracked
						context.Attach(item);
					}

					// Mark the item as modified to ensure the changes are saved
					context.Entry(item).State = EntityState.Modified;
				}
			}

			await context.SaveChangesAsync();


			return Task.FromResult(Task.CompletedTask.IsCompletedSuccessfully).Result;
		}

	}
}
