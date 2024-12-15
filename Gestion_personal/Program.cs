using Blazored.Modal;
using Gestion_personal.Components;
using GestionPersonnel.Services;
using Services;
using Services.Interfaces;
using GestionPersonnel.Storages.PointagesStorages;
using GestionPersonnel.Storages.SalairesStorages;
using GestionPersonnel.Storages.FonctionsStorages;
using Infrastructures.Storages.EmployeStorages;
using GestionPersonnel.Storages.TypeDePaimentStorages;
using GestionPersonnel.Storages.SalairesBaseStorages;
using Implementation.Services.SalaireBase;
using GestionPersonnel.Storages.EquipeStorages;
using GestionPersonnel.Services.EquipeServices;
using GestionPersonnel.Storages.EmployeeEquipeStorages;
using GestionPersonnel.Storages.Storages.PostesStorages;
using GestionPersonnel.Storages.AvancesStorages;
using GestionPersonnel.Storages.DettesStorages;




var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();
builder.Services.AddSingleton<IConfiguration>(provider =>
	new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

builder.Services.AddScoped<EmployeStorage>();
builder.Services.AddScoped<FonctionStorage>();
builder.Services.AddScoped<PointageStorage>();
builder.Services.AddScoped<SalaireStorage>();
builder.Services.AddScoped<TypeDePaiementStorage>();
builder.Services.AddScoped<SalaireBaseStorage>();
builder.Services.AddScoped<EquipeStorage>();
builder.Services.AddScoped<EmployeeEquipeStorage> ();
builder.Services.AddScoped<PosteStorage>();
builder.Services.AddScoped<AvanceStorage>();
builder.Services.AddScoped<DetteRestantStorage>();
builder.Services.AddScoped<DetteStorage>();

builder.Services.AddScoped<ITypeDePaiementService, TypeDePaiementService>();
builder.Services.AddScoped<IEmployeService, EmployeService>();
builder.Services.AddScoped<IFonctionService, FonctionService>();
builder.Services.AddScoped<IPointageService, PointageService>();
builder.Services.AddScoped<ISalaireService, SalaireService>();
builder.Services.AddScoped<ISalaireBaseService, SalaireBaseService>();
builder.Services.AddScoped<IPDFService, PDFService>();
builder.Services.AddScoped<IEquipeService, EquipeService>();
builder.Services.AddScoped<IEmployeeEquipeService, EmployeeEquipeService>();
builder.Services.AddScoped<IPosteService, PosteService>();
builder.Services.AddScoped<IAvanceService, AvanceService>();
builder.Services.AddScoped<IDetteService, DetteService>();
builder.Services.AddScoped<IPdfService,PdfService>();
builder.Services.AddScoped<IDetteRestantService, DetteRestantService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
