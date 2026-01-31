#!/bin/bash

BASE_URL="https://localhost:7082" # Adjust port if needed, assuming default https port or check launchSettings
# Actually, I should check the running port. I'll assume http://localhost:5246 or similar from dotnet run output. 
# Wait, previous run didn't show port clearly but usually it's http://localhost:5246 and https://localhost:7082.
# I will try to grep it from the output or just assume standard.
# Checking log output "Now listening on: https://localhost:7294"
BASE_URL="https://localhost:7294"

EMAIL="admin_jwt_test_$(date +%s)@example.com"
PASSWORD="Password123!"

echo "1. Registering Admin User: $EMAIL"
curl -s -k -L -X POST "$BASE_URL/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d "{
    \"firstName\": \"Admin\",
    \"lastName\": \"User\",
    \"email\": \"$EMAIL\",
    \"password\": \"$PASSWORD\",
    \"role\": \"Admin\"
  }" > register_response.json

cat register_response.json
echo ""

echo "2. Logging in..."
LOGIN_RESPONSE=$(curl -s -k -L -X POST "$BASE_URL/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d "{
    \"email\": \"$EMAIL\",
    \"password\": \"$PASSWORD\"
  }")

TOKEN=$(echo "$LOGIN_RESPONSE" | grep -o '"token":"[^"]*' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
  echo "Failed to get token"
  echo "Login Response: $LOGIN_RESPONSE"
  exit 1
fi

echo "Got Token: ${TOKEN:0:20}..."

echo "3. Accessing Protected Endpoint (GET /api/authors)"
HTTP_CODE=$(curl -s -k -L -o /dev/null -w "%{http_code}" -X GET "$BASE_URL/api/authors?PageNumber=1&PageSize=10" \
  -H "Authorization: Bearer $TOKEN")

echo "Response Code: $HTTP_CODE"

if [ "$HTTP_CODE" -eq 200 ]; then
  echo "SUCCESS: JWT Authentication works!"
else
  echo "FAILURE: Expected 200, got $HTTP_CODE"
  exit 1
fi
