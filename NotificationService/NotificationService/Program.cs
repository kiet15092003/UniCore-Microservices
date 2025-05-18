using NotificationService;
using UserService.CommunicationTypes;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddCommunicationTypes();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
