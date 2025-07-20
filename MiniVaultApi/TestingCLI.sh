#!/bin/bash

# Test MiniVault Minimal API - CLI Script
# Author: [Your Name]
# Date: [Date]

BASE_URL="http://localhost:5111"

echo ""
echo "==============================="
echo "🚀 Testing /generate endpoint"
echo "==============================="
echo "Prompt: tell me about Taj Mahal"
echo ""

curl -s -X POST "$BASE_URL/generate" \
  -H "Content-Type: application/json" \
  -d '{"prompt":"tell me about Taj Mahal"}'

echo ""
echo "/generate test completed."
echo ""

echo "==============================="
echo "🌊 Testing /generate/stream endpoint"
echo "==============================="
echo "Prompt: tell me about Mount Everest"
echo "Streaming response (token by token):"
echo ""

# -N disables buffering for streaming output
curl -N -X POST "$BASE_URL/generate/stream" \
  -H "Content-Type: application/json" \
  -d '{"prompt":"tell me about Mount Everest"}'

echo ""
echo "/generate/stream test completed."
echo ""
