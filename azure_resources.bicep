param appName string
param location string = resourceGroup().location
param sqlAdminUser string = 'SADHANA'
param sqlAdminPassword string

// storage accounts must be between 3 and 24 characters in length and use numbers and lower-case letters only
var storageAccountName = '${substring(appName,0,10)}${uniqueString(resourceGroup().id)}' 
var hostingPlanName = '${appName}${uniqueString(resourceGroup().id)}'
var appInsightsName = '${appName}${uniqueString(resourceGroup().id)}'
var functionAppName = '${appName}'
var loggingStorageAccountName = '${substring(appName,0,7)}log${uniqueString(resourceGroup().id)}'
var sqlServerName = '${appName}sqlserver'
var sqlDbName = '${appName}sqldb'
var serviceBusName = '${appName}servicebus'


resource storageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource loggingStorageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: loggingStorageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}


resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: { 
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
  tags: {
    // circular dependency means we can't reference functionApp directly  /subscriptions/<subscriptionId>/resourceGroups/<rg-name>/providers/Microsoft.Web/sites/<appName>"
     'hidden-link:/subscriptions/${subscription().id}/resourceGroups/${resourceGroup().name}/providers/Microsoft.Web/sites/${functionAppName}': 'Resource'
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2020-10-01' = {
  name: hostingPlanName
  location: location
  sku: {
    name: 'Y1' 
    tier: 'Dynamic'
  }
}

resource functionApp 'Microsoft.Web/sites@2020-06-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    httpsOnly: true
    serverFarmId: hostingPlan.id
    siteConfig: {
      appSettings: [
        {

          'name': 'APPINSIGHTS_INSTRUMENTATIONKEY'
          'value': appInsights.properties.InstrumentationKey
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
           value: '1' 
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        {
          name: 'LoggingStorageAccount'
          value: 'DefaultEndpointsProtocol=https;AccountName=${loggingStorageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(loggingStorageAccount.id, loggingStorageAccount.apiVersion).keys[0].value}'
        }
        {
          'name': 'FUNCTIONS_EXTENSION_VERSION'
          'value': '~4'
        }
        {
          'name': 'FUNCTIONS_WORKER_RUNTIME'
          'value': 'dotnet'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        {
          name: 'FileRepositoryFilePath'
          value: '/home/site/wwwroot/greeting.json'
        }
        {
          name: 'GreetingDbConnectionString'
          value: 'Data Source=tcp:${reference(sqlServer.id).fullyQualifiedDomainName},1433;Initial Catalog=${sqlDbName};User Id=${sqlAdminUser};Password=\'${sqlAdminPassword}\';'
        }
        {
          name:'ServiceBusConnectionString'
          value: listKeys('${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey', serviceBusNamespace.apiVersion).primaryConnectionString
        }
        // WEBSITE_CONTENTSHARE will also be auto-generated - https://docs.microsoft.com/en-us/azure/azure-functions/functions-app-settings#website_contentshare
        // WEBSITE_RUN_FROM_PACKAGE will be set to 1 by func azure functionapp publish
      ]
    }
  }
}

resource sqlServer 'Microsoft.Sql/servers@2019-06-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminUser
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
  }

  resource allowAllWindowsAzureIps 'firewallRules@2021-05-01-preview' = {
    name: 'AllowAllWindowsAzureIps'
    properties: {
      endIpAddress: '0.0.0.0'
      startIpAddress: '0.0.0.0'
    }
  }

  resource sqlDb 'databases@2019-06-01-preview' = {
    name: sqlDbName
    location: location
    sku: {
      name: 'Basic'
      tier: 'Basic'
      capacity: 5
    }
  }
}
resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2018-01-01-preview' = {
  name: serviceBusName
  location: location
  sku: {
    name: 'Standard'
  }

  resource mainTopic 'topics@2021-06-01-preview' = {
    name: 'main'

    resource greetingCreateSubscription 'subscriptions@2021-06-01-preview' = {
      name: 'greeting_create'

      resource rule 'rules@2021-06-01-preview' = {
        name: 'subject'
        properties: {
          correlationFilter: {
            label: 'NewGreeting'
          }
          filterType: 'CorrelationFilter'
        }
      }      
    }
    resource greetingUpdateSubscription 'subscriptions@2021-06-01-preview' = {
      name: 'greeting_update'

      resource rule 'rules@2021-06-01-preview' = {
        name: 'subject'
        properties: {
          correlationFilter: {
            label: 'updategreeting'
          }
          filterType: 'CorrelationFilter'
        }
      }      
    }
  }  
}


