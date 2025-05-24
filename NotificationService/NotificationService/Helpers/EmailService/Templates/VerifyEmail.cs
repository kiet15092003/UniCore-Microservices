using StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;

namespace NotificationService.Helpers.EmailService.Templates
{
    public static class VerifyEmail
    {
        public static string GenerateTemplate(UserImportedEventDataSingleData user)
        {
            return $@"
            <!DOCTYPE html>
            <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>University Student Account Created</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            background-color: #fff8f0;
                        }}
                        .container {{
                            border: 1px solid #f5c28b;
                            border-radius: 5px;
                            padding: 20px;
                            background-color: #fff3e0;
                        }}
                        .header {{
                            text-align: center;
                            margin-bottom: 20px;
                            padding-bottom: 10px;
                            border-bottom: 2px solid #ff9800;
                        }}
                        .credentials {{
                            background-color: #ffe0b2;
                            padding: 15px;
                            border-radius: 5px;
                            margin: 15px 0;
                        }}
                        .footer {{
                            margin-top: 30px;
                            font-size: 0.9em;
                            color: #777;
                            text-align: center;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>Welcome to Our University Portal</h2>
                        </div>
                        <p>Dear Student,</p>
                        <p>Your student account has been successfully created in the university's system.</p>
                        <div class='credentials'>
                            <p><strong>Email Address:</strong> {user.UserEmail}</p>
                            <p><strong>Password:</strong> {user.Password}</p>
                        </div>
                        <p>You can use this account to access academic resources, schedules, and campus information.</p>
                        <p>For security, please change your password after logging in for the first time.</p>
                        <p>If you didn’t register or believe this was sent in error, please contact the university IT support team immediately.</p>
                        <p>Welcome aboard and best wishes for your studies!</p>
                        <div class='footer'>
                            <p>This is an automated message from the university system. Please do not reply to this email.</p>
                        </div>
                    </div>
                </body>
            </html>";
        }
    }
}
