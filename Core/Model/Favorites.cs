using Core.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Core.Model
{
    public class Favorites : IEntity
    {
        public int FavoritesId { get; set; }
        public int UserId { get; private set; }
        public int CarId { get; private set; }
        public DateTime CreatedAt { get; set; }

        public Users? User { get; set; }
        public Car? Cars { get; set; }

        public Favorites() { }

        public Favorites(int userId, int carsId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("O UserId deve ser um ID válido (maior que 0).", nameof(userId));
            }

            if (carsId <= 0)
            {
                throw new ArgumentException("O CarsId deve ser um ID válido (maior que 0).", nameof(carsId));
            }

            CreatedAt = DateTime.UtcNow;

            UserId = userId;
            CarId = carsId;
            CreatedAt = DateTime.UtcNow;
        }

        [SetsRequiredMembers]
        public Favorites(int id, int userId, int carsId, DateTime createdAt)
        {
            if (id < 0)
            {
                throw new ArgumentException("O ID não pode ser negativo.", nameof(id));
            }

            FavoritesId = id;
            UserId = userId;
            CarId = carsId;
            CreatedAt = createdAt;
        }

        public bool IsValid()
        {
            return UserId > 0 && CarId > 0;
        }

        public int GetId() => FavoritesId;

        public void SetId(int id)
        {
            if (FavoritesId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade que já possui um ID.");
            }
            FavoritesId = id;
        }

        public bool GetIsActive() => true;
    }
}
