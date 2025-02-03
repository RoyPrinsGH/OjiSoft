Configuring the VM:

--- Azure ---
- Point DNS to the VM and add HTTP/HTTPS to inbound rules

--- Proxy ---
- Install nginx (do not run)
- Install certbot & certificates (sudo certbot certonly --manual --preferred-challenges dns -d ojisoft.com -d *.ojisoft.com)
- Add ojisoft-idp.conf to the nginx configs
- Start nginx

--- Database ---
- Install mysql and enable
- Copy the production appsettings onto the VM (~/config/idp/appsettings.json)
- Ensure the account from the production appsettings exists in mysql

--- App ---
- Install dotnet
- Add ojisoft-identity-provider.service and enable
