## Dependencies

Ensure you have Docker installed locally.  This was developed on an Ubuntu 22.04 VM. 
To call the API and view JSON output, you should additionally have a modern web browser or curl installed.

## Starting up the API

To start the application, follow these steps:

### Build:
```
docker build -t santander-news-api .
```

### Run:
```
docker run -d -p 5000:8080 --name sna-container santander-news-api
```

### Test call:
```
curl http://localhost:5000/v0/beststories/10
```

## Stopping and cleanup:
```
docker stop sna-container && docker rm sna-container
```

## Assumptions:
I assume that a 1 minute cache expiry for the main story id array will protect the hacker news API from this API, whilst servicing large request numbers.  Due to the fact that most of the story fields shouldn't change (except comment number and score) - a 5 minute cache expiry will also prevent excessive hits on hacker news API.

## Enhancements:
If I had additional time, I would investigate the use of HybridCache to prevent a minor stampede situation where up to 20 (thanks to the use of SemaphoreSlim) concurrent requests may hit hacker news API in the case of expiry/cold cache and barrage of simutaneous traffic.  I believe HybridCache could further restrict that stampede down to 1 request to Hacker News API for the story id array.  

Additionally, I would implement testing. I would start with looking into the features of cucumber and how to develop testing with this tool.

## Bonus command history:
```
sudo apt update
sudo apt install -y docker.io docker-compose-v2
sudo apt install fzf bat silversearcher-ag findutils tree xclip
sudo usermod -aG docker $USER
docker --version
sudo apt install curl
wget https://dot.net/v1/dotnet-install.sh
chmod u+x dotnet-install.sh 
./dotnet-install.sh --channel 10.0
dotnet --version
dotnet new web -o SantanderNewsApi
cd ~/Projects/SantanderNewsApi/
dotnet build
dotnet watch
wget "https://code.visualstudio.com/sha/download?build=stable&os=linux-deb-x64"
sudo apt install ./code_1.141.0-1791276758_amd64.deb -y
mkdir -p ~/.config/Code/User/
cat <<EOF > ~/.config/Code/User/settings.json
{
"telemetry.telemetryLevel": "off",
"workbench.enableExperiments": false,
"extensions.autoCheckUpdates": true,
"update.mode": "manual"
}
EOF
batcat ~/.config/Code/User/settings.json 
echo $DOTNET_CLI_TELEMETRY_OPTOUT
docker build -t santander-news-api .
docker run -d -p 5000:8080 --name sna-container santander-news-api
curl http://localhost:5000/v0/beststories/10
docker logs sna-container | less
docker stop sna-container && docker rm sna-container
```