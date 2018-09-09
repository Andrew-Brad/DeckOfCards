aws --version

# If you're using the CLI for the first time or want to confirm subscription/Identity information
# Type in key and secret
# Default region is us-east-2
# Format can be json, text, table
aws configure


# Messaging:

aws sns list-topics
aws sns list-subscriptions
aws sns list-subscriptions-by-topic --topic-arn <value>

# SNS

# Topic names must be made up of only uppercase and lowercase ASCII letters, numbers, underscores, and 
# hyphens, and must be between 1 and 256 characters long.
# Topic Arn will be returned whether the resource already exists or is created new

aws sns create-topic --name Api-Kickstart


# SQS

aws sqs list-queues
aws sws get-queue-url --queue-name <value>

# The name of the new queue. The following limits apply to this name:
# A queue name can have up to 80 characters.
# Valid values: alphanumeric characters, hyphens (- ), and underscores (_ ).
# A FIFO queue name must end with the .fifo suffix.

aws sqs create-queue --queue-name Api-Kickstart

# Lastly, instruct SNS to subscribe the new SQS endpoint (odd that this goes through sns command)

aws sns subscribe --topic-arn <value> --protocol sqs --notification-endpoint <sqsUrl>

# Might have to confirm subscription? https://docs.aws.amazon.com/cli/latest/reference/sns/confirm-subscription.html


# Deletes:

# SQS Docs: When you delete a queue, you must wait at least 60 seconds before creating a queue with the same name.
aws sqs delete-queue --queue-url https://sqs.us-east-2.amazonaws.com/507925708929/Api-Kickstart

# Keep in mind the subscription will be left dangling pointing at the old Url, so make sure to delete that too
aws sns unsubscribe --subscription-arn arn:aws:sns:us-east-2:507925708929:Api-Kickstart:94fa0a1a-5eca-4581-8d1e-f209ccce0182

# Delete the topic
aws sns delete-topic --topic-arn arn:aws:sns:us-east-2:507925708929:Api-Kickstart
