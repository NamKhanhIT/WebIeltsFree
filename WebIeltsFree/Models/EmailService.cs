using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebIeltsFree.Models
{

    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string email, string token, string resetUrl);
        Task<bool> SendWelcomeEmailAsync(string email, string fullName);
        Task<bool> SendStudyReminderAsync(string email, string fullName);
        Task<bool> SendAchievementNotificationAsync(string email, string fullName, string achievement);
        Task<bool> SendGenericEmailAsync(string email, string subject, string htmlBody);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _smtpHost = EnvHelper.ResolveEnvVars(configuration["Email:SmtpHost"]) ?? "smtp.gmail.com";
            _smtpPort = int.Parse(EnvHelper.ResolveEnvVars(configuration["Email:SmtpPort"]) ?? "587");
            _smtpUsername = EnvHelper.ResolveEnvVars(configuration["Email:SmtpUsername"]) ?? "";
            _smtpPassword = EnvHelper.ResolveEnvVars(configuration["Email:SmtpPassword"]) ?? "";
            _fromEmail = EnvHelper.ResolveEnvVars(configuration["Email:FromEmail"]) ?? "noreply@webieltsfree.com";
            _fromName = EnvHelper.ResolveEnvVars(configuration["Email:FromName"]) ?? "IELTS Learning Platform";
        }



        /// <summary>
        /// Send password reset email
        /// </summary>
        public async Task<bool> SendPasswordResetEmailAsync(string email, string token, string resetUrl)
        {
            try
            {
                var subject = "Reset Your IELTS Password";
                var htmlBody = $@"
                    <h2>Password Reset Request</h2>
                    <p>You requested to reset your password. Click the link below to proceed:</p>
                    <p><a href='{resetUrl}?token={token}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                        Reset Password
                    </a></p>
                    <p>This link expires in 24 hours.</p>
                    <p>If you didn't request this, please ignore this email.</p>
                ";

                return await SendGenericEmailAsync(email, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending password reset email: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send welcome email to new users
        /// </summary>
        public async Task<bool> SendWelcomeEmailAsync(string email, string fullName)
        {
            try
            {
                var subject = "Welcome to IELTS Learning Platform!";
                var htmlBody = $@"
                    <h2>Welcome, {fullName}!</h2>
                    <p>We're excited to have you on your IELTS journey.</p>
                    <h3>Quick Start Guide:</h3>
                    <ol>
                        <li>Take the <strong>Placement Test</strong> to assess your current level</li>
                        <li>Get a personalized <strong>Learning Roadmap</strong> from AI</li>
                        <li>Start with <strong>Reading & Listening</strong> modules</li>
                        <li>Practice <strong>Writing & Speaking</strong> with AI feedback</li>
                        <li>Track your progress with detailed analytics</li>
                    </ol>
                    <p>Happy learning! 🎓</p>
                ";

                return await SendGenericEmailAsync(email, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending welcome email: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send daily study reminder
        /// </summary>
        public async Task<bool> SendStudyReminderAsync(string email, string fullName)
        {
            try
            {
                var subject = "Time to Study! 📚 Daily IELTS Practice";
                var htmlBody = $@"
                    <h2>Hi {fullName},</h2>
                    <p>Don't forget about your IELTS goal! It's time to continue your learning journey.</p>
                    <h3>Today's Recommendation:</h3>
                    <ul>
                        <li>15 minutes of Reading practice</li>
                        <li>10 minutes of Vocabulary builder</li>
                        <li>5 minutes of Speaking exercise</li>
                    </ul>
                    <p>Consistent practice is the key to success. You've got this! 💪</p>
                ";

                return await SendGenericEmailAsync(email, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending study reminder: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send achievement/badge notification
        /// </summary>
        public async Task<bool> SendAchievementNotificationAsync(string email, string fullName, string achievement)
        {
            try
            {
                var subject = $"🎉 Achievement Unlocked: {achievement}!";
                var htmlBody = $@"
                    <h2>Congratulations, {fullName}!</h2>
                    <p>You've unlocked a new achievement: <strong>{achievement}</strong></p>
                    <p>You're making excellent progress on your IELTS journey!</p>
                    <p>Keep up the momentum and continue improving.</p>
                ";

                return await SendGenericEmailAsync(email, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending achievement email: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send generic email
        /// </summary>
        public async Task<bool> SendGenericEmailAsync(string email, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                
                if (!string.IsNullOrEmpty(_smtpUsername))
                {
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"✅ Email sent to {email}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
