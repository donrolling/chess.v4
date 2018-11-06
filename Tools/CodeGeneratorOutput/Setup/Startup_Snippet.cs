services.AddTransient<ISampleRepository, SampleDapperRepository>();
services.AddTransient<ISampleService, SampleService>();
services.AddTransient<IClientRepository, ClientDapperRepository>();
services.AddTransient<IClientService, ClientService>();
