# Learnings
1. Api is an application/x-www-form-urlencoded => found out by using curl and analyzing headers
2. Api is rate limited after ~8ish calls. Presumably sliding window algorithm, however seems its a bit random...
2. Seems like DELETE calls are not guaranteed to work, have to do multiple passes to ensure the board gets wiped.

# Notes

Polly native retries with Flurl are broken right now => no middleware for retries, consumer uses explicitly...

https://stackoverflow.com/questions/40745809/how-to-use-polly-with-flurl-http/77672447#77672447
https://github.com/tmenier/Flurl/issues/346#issuecomment-1848414207
