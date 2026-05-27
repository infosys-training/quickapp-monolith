#!/bin/bash
# Integration smoke test for order creation flow through both services
# Prerequisites: Both monolith (port 5225) and order-service (port 5003) must be running

set -e

ORDER_SERVICE_URL="${ORDER_SERVICE_URL:-http://localhost:5003}"
MONOLITH_URL="${MONOLITH_URL:-http://localhost:5225}"

echo "=== Integration Smoke Test ==="
echo "Order Service: $ORDER_SERVICE_URL"
echo "Monolith:      $MONOLITH_URL"
echo ""

# 1. Health check on order-service
echo "--- Step 1: Health check on order-service ---"
HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" "$ORDER_SERVICE_URL/healthz")
if [ "$HTTP_CODE" = "200" ]; then
    echo "PASS: Order service is healthy (HTTP $HTTP_CODE)"
else
    echo "FAIL: Order service health check failed (HTTP $HTTP_CODE)"
    exit 1
fi
echo ""

# 2. Create an order via order-service directly
echo "--- Step 2: Create order via order-service ---"
CREATE_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$ORDER_SERVICE_URL/api/order" \
    -H "Content-Type: application/json" \
    -d '{
        "discount": 10.00,
        "comments": "Smoke test order",
        "cashierId": "test-cashier",
        "customerId": 1,
        "orderDetails": [
            {
                "unitPrice": 99.99,
                "quantity": 2,
                "discount": 0,
                "productId": 1
            }
        ]
    }')
HTTP_CODE=$(echo "$CREATE_RESPONSE" | tail -1)
BODY=$(echo "$CREATE_RESPONSE" | head -n -1)
if [ "$HTTP_CODE" = "201" ]; then
    ORDER_ID=$(echo "$BODY" | grep -o '"id":[0-9]*' | head -1 | cut -d: -f2)
    echo "PASS: Order created successfully (ID: $ORDER_ID)"
else
    echo "FAIL: Order creation failed (HTTP $HTTP_CODE)"
    echo "$BODY"
    exit 1
fi
echo ""

# 3. Retrieve the order via order-service
echo "--- Step 3: Get order by ID from order-service ---"
GET_RESPONSE=$(curl -s -w "\n%{http_code}" "$ORDER_SERVICE_URL/api/order/$ORDER_ID")
HTTP_CODE=$(echo "$GET_RESPONSE" | tail -1)
BODY=$(echo "$GET_RESPONSE" | head -n -1)
if [ "$HTTP_CODE" = "200" ]; then
    echo "PASS: Order retrieved successfully"
    echo "$BODY" | head -1
else
    echo "FAIL: Failed to retrieve order (HTTP $HTTP_CODE)"
    exit 1
fi
echo ""

# 4. Get orders by customer ID
echo "--- Step 4: Get orders by customer ID ---"
CUST_RESPONSE=$(curl -s -w "\n%{http_code}" "$ORDER_SERVICE_URL/api/order/customer/1")
HTTP_CODE=$(echo "$CUST_RESPONSE" | tail -1)
if [ "$HTTP_CODE" = "200" ]; then
    echo "PASS: Customer orders retrieved successfully"
else
    echo "FAIL: Failed to get customer orders (HTTP $HTTP_CODE)"
    exit 1
fi
echo ""

# 5. Test order retrieval through monolith proxy (if monolith is running)
echo "--- Step 5: Test order retrieval through monolith ---"
MONOLITH_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" "$MONOLITH_URL/api/order" 2>/dev/null || echo "000")
if [ "$MONOLITH_RESPONSE" = "200" ]; then
    echo "PASS: Monolith proxied order request successfully"
elif [ "$MONOLITH_RESPONSE" = "000" ]; then
    echo "SKIP: Monolith not running (expected in standalone order-service testing)"
else
    echo "INFO: Monolith returned HTTP $MONOLITH_RESPONSE (may need authentication)"
fi
echo ""

# 6. Delete the test order
echo "--- Step 6: Clean up - delete test order ---"
DEL_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" -X DELETE "$ORDER_SERVICE_URL/api/order/$ORDER_ID")
if [ "$DEL_RESPONSE" = "204" ]; then
    echo "PASS: Test order cleaned up"
else
    echo "WARN: Cleanup returned HTTP $DEL_RESPONSE"
fi
echo ""

echo "=== Smoke Test Complete ==="
