using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.ReasonCodes;
using HiveMQtt.MQTT5.Types;
using System.Text;

namespace SimpleMqtt;

/// <summary>
/// Deze klasse 'wrapt' de HiveMQClient die alle code bevat om
/// verbinding te maken met MQTT brokers. Omwille van eenvoudig gebruik
/// maken we alleen de relevante opties beschikbaar en verbergen hier complexiteit
/// </summary>
public class SimpleMqttClient : IDisposable
{
    /// <summary>
    /// Berichten worden standaard verstu/// urd en ontvangen in onderstaande text encoding
    /// </summary>
    private static Encoding DefaultEncoding = Encoding.ASCII;

    /// <summary>
    /// Een interne referentie naar de HiveMQClient
    /// </summary>
    private readonly HiveMQClient _client;

    /// <summary>
    /// De constructor
    /// </summary>
    public SimpleMqttClient(SimpleMqttClientConfiguration options)
    {
        this.ClientId = options.ClientId;

        _client = new HiveMQClient(new()
        {
            ClientId = options.ClientId,
            CleanStart = options.CleanStart,
            Port = options.Port,
            Host = options.Host!,
            ConnectTimeoutInMs = options.TimeoutInMs,
            UserName = options.UserName,
            Password = options.Password
        });

        _client.OnMessageReceived += OnHiveMQttMessageRecieved;
    }

    /// <summary>
    /// Methodes die hangen aan dit event worden uitgevoerd 
    /// wanneer er een MQTT bericht wordt ontvangen.
    /// Voorbeeld aanroep:
    /// client.OnMessageReceived += (sender, args) =>
    /// {
    ///    Console.WriteLine($"Bericht ontvangen; topic={args.Topic}; message={args.Message};");
    ///  };
    /// </summary>
    public event EventHandler<SimpleMqttMessage>? OnMessageReceived;

    /// <summary>
    /// Geeft de client id van de MQTT broker
    /// </summary>
    public string? ClientId { get; private set; }

    /// <summary>
    /// Stuur een message naar de broker. Let op het juiste topic! Deze moet uniek zijn op de broker.
    /// </summary>
    /// <param name="message">Het bericht dat verstuurd moet worden</param>
    public async Task PublishMessage(SimpleMqttMessage message)
    {
        await this.OpenAndVerifyConnection();

        var mqttMessage = new MQTT5PublishMessage
        {
            Topic = message.Topic,
            Payload = DefaultEncoding.GetBytes(message.Message!),
            QoS = QualityOfService.ExactlyOnceDelivery,
        };

        var publishResult = await _client.PublishAsync(mqttMessage).ConfigureAwait(false);

        if (publishResult.QoS2ReasonCode != PubRecReasonCode.Success)
        {
            throw new InvalidOperationException($"Unable to publish message [reason code: {publishResult.QoS2ReasonCode.GetValueOrDefault(PubRecReasonCode.UnspecifiedError)}");
        }
    }

    /// <summary>
    /// Stuur een message naar de broker. Let op het juiste topic! Deze moet uniek zijn op de broker.
    /// </summary>
    /// <param name="message">te versturen melding als string</param>
    /// <param name="topic">naam van topic</param>
    public Task PublishMessage(string message, string topic) => PublishMessage(new() { Topic = topic, Message = message });

    /// <summary>
    /// Luistert naar een topic voor nieuwe berichten
    /// </summary>
    public async Task SubscribeToTopic(string topic)
    {
        await this.OpenAndVerifyConnection();
        await _client.SubscribeAsync(topic, QualityOfService.ExactlyOnceDelivery).ConfigureAwait(false);
    }

    /// <summary>
    /// Wrapper om het HiveMQtt bericht
    /// </summary>
    private void OnHiveMQttMessageRecieved(object? sender, OnMessageReceivedEventArgs e)
    {
        // Trigger the new wrapped event with custom event arguments
        var msg = new SimpleMqttMessage
        {
            Topic = e.PublishMessage.Topic!,
            Message = DefaultEncoding.GetString(e.PublishMessage.Payload!)
        };

        this.OnMessageReceived?.Invoke(this, msg);
    }

    /// <summary>
    /// Opent de verbinding met de broker
    /// </summary>
    /// <returns>true als de verbinding goed is geopend</returns>
    private async Task OpenAndVerifyConnection()
    {
        // Open de verbinding wanneer deze niet open is
        if (!this._client.IsConnected())
        {
            var connectionResult = await _client.ConnectAsync().ConfigureAwait(false);

            if (connectionResult.ReasonCode != ConnAckReasonCode.Success)
            {
                throw new InvalidOperationException($"Failed to connect: {connectionResult.ReasonString}");
            }
        }
    }

    /// <summary>
    /// Wanneer het object expliciet wordt weggegooid sluiten we de connectie
    /// </summary>
    public void Dispose() => _client.Dispose();

    /// <summary>
    /// Wanneer het object wordt opgeruimd door de GC sluiten we de connectie
    /// </summary>
    ~SimpleMqttClient() => _client.Dispose();

    /// <summary>
    /// Maakt een instantie van een SimpleMqttClient geschikt voor gebruik met HiveMQ 
    /// </summary>
    /// <param name="clientId">Een unieke naam van je client</param>
    /// <returns></returns>
    public static SimpleMqttClient CreateSimpleMqttClientForHiveMQ(string clientId)
    {
        var mqttWrapper = new SimpleMqttClient(new()
        {
            Host = "b81bd6be7cf9423789cd5328ea972f5f.s1.eu.hivemq.cloud", // maak eventueel een account aan bij hivemq als dit problemen geeft.
            Port = 8883,
            CleanStart = false, // <--- false, haalt al gebufferde meldingen ook op.
            ClientId = clientId, // Dit clientid moet uniek zijn binnen de broker
            TimeoutInMs = 5_000, // Standaard time-out bij het maken van een verbinding (5 seconden)
            UserName = "school",
            Password = "Test!2345"
        });

        return mqttWrapper;
    }
}

/// <summary>
/// Deze klasse bevat de instructies waarmee je MQTT client wordt geconfigureerd
/// </summary>
public class SimpleMqttClientConfiguration
{
    /// <summary>
    /// De MQTT host waar je mee wilt verbinden
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Het poort nummer waarover je verbinding wilt maken
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// De identifier van je client. Die moet uniek zijn
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// De timeout in milliseconds die je wilt hanteren voor het maken van een verbinding
    /// </summary>
    public int TimeoutInMs { get; set; }

    /// <summary>
    /// Geeft aan of je reeds gebufferde berichten opnieuw wilt binnenhalen
    /// </summary>
    public bool CleanStart { get; set; }

    /// <summary>
    /// Gebruikersnaam voor de verbinding
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Wachtwoord wat hoort bij de username
    /// </summary>
    public string? Password { get; set; }
}

/// <summary>
/// Deze klasse bevat een eenvoudig MQTT bericht
/// </summary>
public class SimpleMqttMessage
{
    public string? Topic { get; set; }
    public string? Message { get; set; }
}
