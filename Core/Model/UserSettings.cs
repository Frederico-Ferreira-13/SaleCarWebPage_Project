using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Core.Model
{
    public class UserSettings : IEntity
    {
        [Key]
        public int UserSettingId { get; private set; }

        public int UserId { get; protected set; }

        public string Theme { get; protected set; }
        public string Language { get; protected set; }
        public bool ReceiveNotifications { get; protected set; }

        public virtual Users? User { get; private set; }

        [SetsRequiredMembers]
        private UserSettings()
        {
            UserSettingId = default;
            Theme = string.Empty;
            Language = string.Empty;
            ReceiveNotifications = false;
        }

        public UserSettings(int userId, string theme, string language, bool receiveNotifications)
        {
            ValidateSettings(userId, theme, language);           

            UserId = userId;
            Theme = theme;
            Language = language;
            ReceiveNotifications = receiveNotifications;
        }

        public UserSettings(int id, int userId, string theme, string language, bool receiveNotifications)
            : this(userId, theme, language, receiveNotifications)
        {
            UserSettingId = id;
        }

        public void UpdateSettings(string newTheme, string newLanguage, bool newReceiveNotifications)
        {
            ValidateSettings(UserId, newTheme, newLanguage);

            Theme = newTheme;
            Language = newLanguage;
            ReceiveNotifications = newReceiveNotifications;
        }

        private static void ValidateSettings(int userId, [NotNull] string? theme, [NotNull] string? language)
        {
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "O ID do Utilizador deve ser positivo.");
            }
            if (string.IsNullOrWhiteSpace(theme))
            {
                throw new ArgumentException("O tema não pode ser vazio.", nameof(theme));
            }
            if (string.IsNullOrWhiteSpace(language))
            {
                throw new ArgumentException("O idioma não pode ser vazio.", nameof(language));
            }

            if (language.Length > 10)
            {
                throw new ArgumentException("O idioma não pode exceder 10 caracteres (ex: 'pt-PT')", nameof(language));
            }
        }

        public void UpdateTheme(string newTheme)
        {
            if (Theme != newTheme)
            {
                UpdateSettings(newTheme, Language, ReceiveNotifications);
            }
        }

        public void UpdateLanguage(string newLanguage)
        {
            if (Language != newLanguage)
            {
                UpdateSettings(Theme, newLanguage, ReceiveNotifications);
            }
        }

        public void UpdateReceiveNotifications(bool newNotificationsEnabled)
        {
            if (ReceiveNotifications != newNotificationsEnabled)
            {
                UpdateSettings(Theme, Language, newNotificationsEnabled);
            }
        }

        public int GetId() => UserSettingId;

        public void SetId(int id)
        {
            if (UserSettingId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade que já possui um ID.");
            }
            UserSettingId = id;
        }

        public bool GetIsActive() => true;
    }
}
