using System.Collections.Generic;

public abstract class Configureable
{
    public Config Configuration;

    public void Configure(string configPath, List<string> requiredFields)
    {
        Configuration = new Config(requiredFields);

        Configuration.LoadValuesFromFile(configPath);
    }
}