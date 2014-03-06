param (
	[string]$username = $(throw "-username is required")
)

# for partition module
iex "netsh http add urlacl url=http://+:8080/ user=$username"

# for matcher modules
$ports = @(1,2,3,4,5,6,7,8,9,10)
foreach ($p in $ports) {
	iex "netsh http add urlacl url=http://+:$(1200+$p)/ user=$username"
}