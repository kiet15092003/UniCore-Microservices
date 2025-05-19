using NotificationService;
using UserService.CommunicationTypes;
using UserService.CommunicationTypes.Http.HttpClient;
using NotificationService.Helpers.EmailService;
using StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<SmtpClientService>();
builder.Services.AddSingleton<SendEmailVerifyAccountService>();
builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddCommunicationTypes();

// Register Worker as a singleton and as a hosted service
builder.Services.AddSingleton<Worker>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<Worker>());

var host = builder.Build();

host.Run();
